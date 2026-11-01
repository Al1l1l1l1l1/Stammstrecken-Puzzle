#!/usr/bin/env python3
"""Small adversarial Full snapshots; optional actual untouched CI XML readback.

Run with STP_COVERAGE_ORIGINALS=<directory> to include the preserved run39665719215
readback and independently mutated copies. Fixtures do not assert a Unity run.
"""
from __future__ import annotations

import copy
import contextlib
import hashlib
import io
import os
from pathlib import Path
import shutil
import sys
import tempfile
import unittest
import xml.etree.ElementTree as ET

sys.dont_write_bytecode = True
sys.path.insert(0, str(Path(__file__).resolve().parent))
import verify_coverage as subject  # noqa: E402


def point(offset: int, *, line: int = 12, column: int = 0, visits: int = 0) -> ET.Element:
    return ET.Element("SequencePoint", dict(vc=str(visits), uspid="1", ordinal="0", offset=str(offset),
                                           sl=str(line), sc=str(column), el=str(line), ec=str(column), bec="0", bev="0", fileid="1"))


def make_snapshot(*, refinement: bool = False, visited: bool = False) -> ET.Element:
    root = ET.Element("CoverageSession", Version="0.0.0.0")
    ET.SubElement(root, "Summary")
    modules = ET.SubElement(root, "Modules")
    for name in sorted(subject.REQUIRED_MODULES):
        mod = ET.SubElement(modules, "Module")
        ET.SubElement(mod, "Summary")
        ET.SubElement(mod, "ModuleTime").text = "0001-01-01T00:00:00"
        ET.SubElement(mod, "ModuleName").text = name
        files = ET.SubElement(mod, "Files")
        ET.SubElement(files, "File", uid="1", fullPath="/source/" + name + ".cs")
        classes = ET.SubElement(mod, "Classes")
        c = ET.SubElement(classes, "Class")
        ET.SubElement(c, "Summary")
        cn = name + ".Owner/<>c__DisplayClass1_0/<<Work>b__0>d"
        ET.SubElement(c, "FullName").text = cn
        methods = ET.SubElement(c, "Methods")
        method = ET.SubElement(methods, "Method", visited="false", cyclomaticComplexity="0", nPathComplexity="0",
                               sequenceCoverage="0", branchCoverage="0", crapScore="0", isConstructor="false",
                               isStatic="false", isGetter="false", isSetter="false")
        ET.SubElement(method, "Summary")
        ET.SubElement(method, "MetadataToken").text = "100663297"
        ET.SubElement(method, "Name").text = "System.Void " + cn + "::MoveNext()"
        ET.SubElement(method, "FileRef", uid="1")
        seq = ET.SubElement(method, "SequencePoints")
        seq.append(point(10, column=9 if visited else 0, visits=int(visited)))
        seq.append(point(100, line=13, column=20 if visited else 0, visits=int(visited)))
        if refinement and name == "STP.Application":
            seq.insert(1, point(30, column=9, visits=0))
            seq.insert(2, point(60, column=9, visits=0))
        ET.SubElement(method, "BranchPoints")
    recount(root)
    return root


def recount(root: ET.Element) -> None:
    """Independent fixture writer: recompute rows to avoid summary-only rejections."""
    def assign(node: ET.Element, *, method: bool = False, cls: bool = False) -> None:
        seq = list(node.iter("SequencePoint"))
        branch = list(node.iter("BranchPoint"))
        methods = [node] if method else list(node.iter("Method"))
        classes = [] if method else [node] if cls else list(node.iter("Class"))
        values = dict(numSequencePoints=len(seq), visitedSequencePoints=sum(int(p.get("vc", "0")) > 0 for p in seq),
                      numBranchPoints=len(branch), visitedBranchPoints=sum(int(p.get("vc", "0")) > 0 for p in branch),
                      numMethods=len(methods), visitedMethods=sum(m.get("visited") == "true" for m in methods),
                      numClasses=len(classes), visitedClasses=sum(any(m.get("visited") == "true" for m in c.iter("Method")) for c in classes))
        summary = node.find("Summary")
        assert summary is not None
        summary.attrib = {k: str(v) for k, v in values.items()}
        for kind, field in (("SequencePoints", "sequenceCoverage"), ("BranchPoints", "branchCoverage")):
            n, d = values["visited" + kind], values["num" + kind]
            summary.set(field, f"{100 * n / d:.2f}" if d else "0.0")
        if method:
            node.set("sequenceCoverage", summary.get("sequenceCoverage"))
            node.set("branchCoverage", summary.get("branchCoverage"))
    uid = 1
    for p in root.iter():
        if p.tag in {"SequencePoint", "BranchPoint"}:
            p.set("uspid", str(uid))
            uid += 1
    for m in root.iter("Method"):
        m.set("visited", str(any(int(p.get("vc", "0")) > 0 for p in m.iter("SequencePoint"))).lower())
        assign(m, method=True)
    for c in root.iter("Class"):
        assign(c, cls=True)
    for m in root.iter("Module"):
        assign(m)
    assign(root)


class CoverageTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temp = tempfile.TemporaryDirectory(prefix="stp-coverage-", dir=os.environ.get("STP_TEST_TEMP_ROOT"))
        self.addCleanup(self.temp.cleanup)
        self.base = Path(self.temp.name)
        self.before = make_snapshot()
        self.after = make_snapshot(refinement=True, visited=True)

    def write(self, before=None, after=None) -> None:
        ET.ElementTree(self.before if before is None else before).write(self.base / "TestCoverageResults_0000.xml", encoding="utf-8", xml_declaration=True)
        ET.ElementTree(self.after if after is None else after).write(self.base / "TestCoverageResults_0001.xml", encoding="utf-8", xml_declaration=True)

    def method(self, root=None):
        root = self.after if root is None else root
        return next(m for m in root.iter("Module") if m.findtext("ModuleName") == "STP.Application").find("./Classes/Class/Methods/Method")

    def reject(self, match: str, *, recount_after: bool = True) -> None:
        if recount_after:
            recount(self.after)
        self.write()
        with self.assertRaisesRegex(subject.CoverageError, match):
            subject.verify(self.base)

    def test_final_full_is_named_denominator_and_zero_visited_is_valid(self):
        self.write()
        report = subject.verify(self.base)
        module = next(m for m in report["modules"] if m["name"] == "STP.Application")
        self.assertEqual("final Full actual row inventory", report["denominator"])
        self.assertEqual((2, 4, 2, 2), (module["baselineSequencePoints"], module["finalFullSequencePointsDenominator"], module["visitedSequencePoints"], module["addedAsyncSequencePoints"]))
        self.assertIsNone(report["minimumCoverage"])

    def test_identical_zero_coverage_full_inventory_passes(self):
        self.write(after=copy.deepcopy(self.before))
        self.assertTrue(all(m["visitedSequencePoints"] == 0 for m in subject.verify(self.base)["modules"]))

    def test_source_columns_can_fill_only_previously_zero_columns(self):
        self.after = make_snapshot(visited=True)
        self.write()
        self.assertEqual(2, subject.verify(self.base)["modules"][0]["resolvedColumnPoints"])
        self.method(self.before).find("./SequencePoints/SequencePoint").set("sc", "2")
        self.method(self.before).find("./SequencePoints/SequencePoint").set("ec", "2")
        self.reject("nonzero columns")

    def test_missing_directory_or_empty_evidence_is_not_zero(self):
        with self.assertRaisesRegex(subject.CoverageError, "missing"):
            subject.verify(self.base / "absent")
        with self.assertRaisesRegex(subject.CoverageError, "no TestCoverage"):
            subject.verify(self.base)

    def test_initial_and_exactly_one_final_full_are_required(self):
        self.write()
        (self.base / "TestCoverageResults_0001.xml").unlink()
        with self.assertRaisesRegex(subject.CoverageError, "exactly initial"):
            subject.verify(self.base)
        self.write()
        shutil.copyfile(self.base / "TestCoverageResults_0001.xml", self.base / "TestCoverageResults_0002.xml")
        with self.assertRaisesRegex(subject.CoverageError, "exactly initial"):
            subject.verify(self.base)

    def test_single_session_and_supported_filename_required(self):
        self.write()
        other = self.base / "other"
        other.mkdir()
        shutil.move(self.base / "TestCoverageResults_0001.xml", other)
        with self.assertRaisesRegex(subject.CoverageError, "multiple coverage sessions"):
            subject.verify(self.base)
        shutil.move(other / "TestCoverageResults_0001.xml", self.base)
        (self.base / "TestCoverageResults_wrong.xml").write_text("bad", encoding="utf-8")
        with self.assertRaisesRegex(subject.CoverageError, "filename"):
            subject.verify(self.base)

    def test_missing_baseline_or_null_modules_is_closed(self):
        self.write()
        (self.base / "TestCoverageResults_0000.xml").unlink()
        with self.assertRaisesRegex(subject.CoverageError, "exactly initial"):
            subject.verify(self.base)
        self.write()
        (self.base / "TestCoverageResults_0001.xml").write_text("<CoverageSession><Modules/></CoverageSession>", encoding="utf-8")
        with self.assertRaises(subject.CoverageError):
            subject.verify(self.base)

    def test_duplicate_module_class_method_file_and_point_fail(self):
        for tag in ("Module", "Class", "Method", "File", "SequencePoint"):
            with self.subTest(tag=tag):
                self.after = make_snapshot(refinement=True, visited=True)
                node = next(self.after.iter(tag))
                parent = next(p for p in self.after.iter() if node in list(p))
                parent.append(copy.deepcopy(node))
                self.reject("duplicate|Summary")

    def test_missing_required_module_even_from_both_is_not_omitted(self):
        for root in (self.before, self.after):
            modules = root.find("Modules")
            modules.remove(next(m for m in modules if m.findtext("ModuleName") == "STP.Puzzle.Domain"))
            recount(root)
        self.reject("required module")

    def test_missing_or_extra_final_module_fails(self):
        self.after.find("Modules").remove(self.after.find("./Modules/Module"))
        self.reject("module inventory")

    def test_changed_file_map_even_unused_row_fails(self):
        self.after.find("./Modules/Module/Files/File").set("fullPath", "/changed/source.cs")
        self.reject("File map")

    def test_changed_method_name_token_flags_and_module_metadata_fail(self):
        for field in ("name", "token", "flag", "time"):
            with self.subTest(field=field):
                self.after = make_snapshot(refinement=True, visited=True)
                m = self.method()
                if field == "name":
                    m.find("Name").text += "changed"
                elif field == "token":
                    m.find("MetadataToken").text = "100663300"
                elif field == "flag":
                    m.set("isStatic", "true")
                else:
                    self.after.find("./Modules/Module/ModuleTime").text = "changed"
                self.reject("metadata|method inventory")

    def test_deleted_original_point_and_changed_offset_line_file_fail(self):
        for field in ("remove", "offset", "sl", "fileid"):
            with self.subTest(field=field):
                self.after = make_snapshot(refinement=True, visited=True)
                seq = self.method().find("SequencePoints")
                p = seq[0]
                if field == "remove":
                    seq.remove(p)
                else:
                    p.set(field, "999")
                self.reject("offsets removed|offset/file/line|source range|FileRef")

    def test_normal_method_cannot_gain_offsets(self):
        for root in (self.before, self.after):
            m = self.method(root)
            m.find("Name").text = "System.Void STP.Application.Owner::Run()"
        self.reject("outside generated async")

    def test_generated_name_without_matching_class_is_not_evidence(self):
        for root in (self.before, self.after):
            mod = next(m for m in root.iter("Module") if m.findtext("ModuleName") == "STP.Application")
            mod.find("./Classes/Class/FullName").text = "STP.Application.NormalClass"
        self.reject("outside generated async")

    def test_new_offset_outside_original_range_is_rejected(self):
        self.method().find("SequencePoints")[1].set("offset", "101")
        self.reject("outside original method range")

    def test_new_point_requires_original_resolved_column_anchor(self):
        for root in (self.before, self.after):
            seq = self.method(root).find("SequencePoints")
            if root is self.before:
                seq[0].set("sc", "9")
                seq[0].set("ec", "9")
        self.reject("anchor refinement")

    def test_new_point_cannot_add_a_new_source_span_inside_method_lines(self):
        self.method().find("SequencePoints")[1].set("sc", "10")
        self.method().find("SequencePoints")[1].set("ec", "10")
        self.reject("anchor refinement")

    def test_one_or_three_new_offsets_or_mismatched_pair_visits_rejected(self):
        for kind in ("one", "three", "visits"):
            with self.subTest(kind=kind):
                self.after = make_snapshot(refinement=True, visited=True)
                seq = self.method().find("SequencePoints")
                if kind == "one":
                    seq.remove(seq[1])
                elif kind == "three":
                    seq.insert(2, point(40, column=9))
                else:
                    seq[1].set("vc", "1")
                self.reject("offset-pair shape")

    def test_no_refinement_without_resolved_columns(self):
        for p in self.method().iter("SequencePoint"):
            p.set("sc", "0")
            p.set("ec", "0")
        self.reject("unsupported async refinement")

    def test_module_class_method_and_session_summary_are_exact_row_sums(self):
        for tag in ("CoverageSession", "Module", "Class", "Method"):
            for field in subject.COUNTERS:
                with self.subTest(tag=tag, field=field):
                    self.after = make_snapshot(refinement=True, visited=True)
                    node = self.after if tag == "CoverageSession" else next(self.after.iter(tag))
                    s = node.find("Summary")
                    s.set(field, str(int(s.get(field)) + 1))
                    self.reject("Summary", recount_after=False)

    def test_summary_null_negative_noninteger_or_missing_are_rejected(self):
        for value in (None, "-1", "1.0", "NaN", ""):
            with self.subTest(value=value):
                self.after = make_snapshot(refinement=True, visited=True)
                s = self.after.find("./Modules/Module/Summary")
                if value is None:
                    del s.attrib["numMethods"]
                else:
                    s.set("numMethods", value)
                self.reject("invalid/missing count", recount_after=False)

    def test_vc_negative_missing_nonnumeric_and_visited_flag_mismatch_fail(self):
        for value in (None, "-1", "NaN", "0.5"):
            with self.subTest(value=value):
                self.after = make_snapshot(refinement=True, visited=True)
                p = self.method().find("./SequencePoints/SequencePoint")
                if value is None:
                    del p.attrib["vc"]
                else:
                    p.set("vc", value)
                self.reject("shape|invalid/missing count", recount_after=False)
        self.after = make_snapshot(refinement=True, visited=True)
        self.method().set("visited", "false")
        self.reject("visited flag contradicts", recount_after=False)

    def test_percentage_is_rendering_only_but_nan_infinite_or_wrong_ratio_fail(self):
        for value in ("NaN", "Infinity", "-1", "101", "0"):
            with self.subTest(value=value):
                self.after = make_snapshot(refinement=True, visited=True)
                self.after.find("./Modules/Module/Summary").set("sequenceCoverage", value)
                self.reject("percentage", recount_after=False)

    def test_duplicate_uspid_and_duplicate_summary_fail(self):
        self.method().find("SequencePoints")[1].set("uspid", self.method().find("SequencePoints")[0].get("uspid"))
        self.reject("duplicate/invalid point id", recount_after=False)
        self.after = make_snapshot(refinement=True, visited=True)
        self.method().append(copy.deepcopy(self.method().find("Summary")))
        self.reject("exactly one Summary", recount_after=False)

    def test_partial_class_other_mapped_file_is_kept_exactly(self):
        for root in (self.before, self.after):
            mod = next(m for m in root.iter("Module") if m.findtext("ModuleName") == "STP.Application")
            ET.SubElement(mod.find("Files"), "File", uid="2", fullPath="/partial/other.cs")
            self.method(root).find("FileRef").set("uid", "2")
        self.write()
        subject.verify(self.base)

    def test_zero_point_method_is_not_usable(self):
        self.method().find("SequencePoints").clear()
        self.reject("no sequence points")

    def test_branch_rows_visit_counts_and_identity_are_verified(self):
        self.after = make_snapshot(visited=True)
        for root in (self.before, self.after):
            ET.SubElement(self.method(root).find("BranchPoints"), "BranchPoint", vc="1" if root is self.after else "0",
                          uspid="1", ordinal="0", offset="10", sl="12", path="0", offsetend="50", fileid="1")
            recount(root)
        self.write()
        subject.verify(self.base)
        self.method().find("./BranchPoints/BranchPoint").set("offsetend", "51")
        self.reject("branch inventory changed")

    def test_branch_exit_counts_cannot_exceed_total_or_change_original_shape(self):
        self.method().find("SequencePoints")[0].set("bev", "1")
        self.reject("visited branch exits")
        self.after = make_snapshot(refinement=True, visited=True)
        self.method().find("SequencePoints")[0].set("bec", "1")
        self.reject("offset/file/line/range changed")

    def test_extra_method_even_with_valid_new_token_is_rejected(self):
        extra = copy.deepcopy(self.method())
        extra.find("Name").text += "Extra"
        extra.find("MetadataToken").text = "100663399"
        parent = next(p for p in self.after.iter("Methods") if self.method() in list(p))
        parent.append(extra)
        self.reject("method inventory")

    def test_unknown_nested_points_and_missing_method_data_are_not_ignored(self):
        for kind in ("unknown", "token", "fileref", "name", "summary"):
            with self.subTest(kind=kind):
                self.after = make_snapshot(refinement=True, visited=True)
                m = self.method()
                if kind == "unknown":
                    ET.SubElement(m, "UncountedPoints").append(point(77))
                else:
                    tag = {"token": "MetadataToken", "fileref": "FileRef", "name": "Name", "summary": "Summary"}[kind]
                    m.remove(m.find(tag))
                self.reject("shape|exactly one", recount_after=False)

    def test_initial_is_unvisited_not_another_measurement(self):
        self.before = make_snapshot(visited=True)
        self.reject("not an unvisited")

    def test_invalid_xml_wrong_root_and_entities_fail(self):
        for value in ("<", "<Other/>", '<!DOCTYPE X [<!ENTITY x "x">]><CoverageSession/>'):
            with self.subTest(value=value):
                self.write()
                (self.base / "TestCoverageResults_0001.xml").write_text(value, encoding="utf-8")
                with self.assertRaises(subject.CoverageError):
                    subject.verify(self.base)

    def test_cli_report_is_exclusive_and_original_bytes_are_not_written(self):
        self.write()
        original = {p.name: p.read_bytes() for p in self.base.glob("*.xml")}
        report = self.base / "report.json"
        with contextlib.redirect_stdout(io.StringIO()), contextlib.redirect_stderr(io.StringIO()):
            self.assertEqual(0, subject.main([str(self.base), "--report", str(report)]))
            self.assertEqual(1, subject.main([str(self.base), "--report", str(report)]))
            self.assertEqual(1, subject.main([str(self.base), "--report", str(self.base / "TestCoverageResults_0001.xml")]))
        self.assertEqual(original, {p.name: p.read_bytes() for p in self.base.glob("*.xml")})

    @unittest.skipUnless(os.environ.get("STP_COVERAGE_ORIGINALS"), "preserved actual CI XML supplied explicitly")
    def test_actual_ci_originals_and_tight_mutations(self):
        directory = Path(os.environ["STP_COVERAGE_ORIGINALS"])
        files = sorted(directory.rglob("TestCoverageResults_*.xml"))
        hashes = {p: hashlib.sha256(p.read_bytes()).hexdigest() for p in files}
        report = subject.verify(directory)
        self.assertEqual(22, len(report["modules"]))
        changed = [r for m in report["modules"] for r in m["refinements"] if r["addedAsyncOffsets"]]
        self.assertEqual((5, 20), (len(changed), sum(len(c["addedAsyncOffsets"]) for c in changed)))
        app = next(m for m in report["modules"] if m["name"] == "STP.Application")
        self.assertEqual((6803, 6821, 4747), (app["baselineSequencePoints"], app["finalFullSequencePointsDenominator"], app["visitedSequencePoints"]))
        target = self.base / "actual"
        target.mkdir()
        for p in files:
            shutil.copyfile(p, target / p.name)
        final = next(target.glob("*0001.xml"))
        original_final = final.read_bytes()
        for mutation in ("summary", "oldpoint", "token", "newspan", "newpair"):
            with self.subTest(mutation=mutation):
                root = ET.fromstring(original_final)
                method = next(m for m in root.iter("Method") if m.findtext("Name") == changed[0]["method"])
                if mutation == "summary":
                    s = root.find("Summary")
                    s.set("numSequencePoints", str(int(s.get("numSequencePoints")) - 20))
                elif mutation == "oldpoint":
                    p = method.find("SequencePoints")[0]
                    p.set("sl", "160")
                    p.set("el", "160")
                elif mutation == "token":
                    method.find("MetadataToken").text = "100669999"
                elif mutation == "newspan":
                    method.find("SequencePoints")[2].set("sc", "96")
                    method.find("SequencePoints")[2].set("ec", "96")
                else:
                    method.find("SequencePoints").remove(method.find("SequencePoints")[2])
                    recount(root)
                ET.ElementTree(root).write(final, encoding="utf-8", xml_declaration=True)
                with self.assertRaises(subject.CoverageError):
                    subject.verify(target)
        self.assertEqual(hashes, {p: hashlib.sha256(p.read_bytes()).hexdigest() for p in files})


if __name__ == "__main__":
    unittest.main(verbosity=2)
