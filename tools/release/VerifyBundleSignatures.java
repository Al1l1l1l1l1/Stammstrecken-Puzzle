import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.security.MessageDigest;
import java.security.CodeSigner;
import java.security.cert.X509Certificate;
import java.util.HashSet;
import java.util.HexFormat;
import java.util.Set;
import java.util.jar.JarEntry;
import java.util.jar.JarFile;

/** Offline cryptographic verification of every payload entry, pinned upload certificate; no keystore access. */
public final class VerifyBundleSignatures {
    public static void main(String[] args) throws Exception {
        if (args.length != 2 || !args[1].matches("[0-9a-f]{64}")) throw new IllegalArgumentException("RC_CERT_INPUT_INVALID");
        int payload = 0;
        int manifests = 0;
        Set<String> names = new HashSet<>();
        Set<String> signatureFiles = new HashSet<>();
        Set<String> signatureBlocks = new HashSet<>();
        try (JarFile jar = new JarFile(args[0], true)) {
            var entries = jar.entries();
            while (entries.hasMoreElements()) {
                JarEntry entry = entries.nextElement();
                if (!names.add(entry.getName())) throw new SecurityException("RC_DUPLICATE_JAR_ENTRY");
                if (entry.isDirectory()) continue;
                try (InputStream stream = jar.getInputStream(entry)) { stream.transferTo(java.io.OutputStream.nullOutputStream()); }
                String upper = entry.getName().toUpperCase(java.util.Locale.ROOT);
                if (upper.equals("META-INF/MANIFEST.MF")) {
                    if (++manifests != 1) throw new SecurityException("RC_EXTRA_SIGNATURE_MANIFEST");
                    continue;
                }
                if (upper.matches("META-INF/[^/]+\\.(RSA|DSA|EC|SF)")) {
                    String stem = upper.substring(0, upper.lastIndexOf('.'));
                    if (upper.endsWith(".SF")) {
                        if (!signatureFiles.add(stem)) throw new SecurityException("RC_EXTRA_SIGNATURE_FILE");
                    }
                    else if (!signatureBlocks.add(stem)) throw new SecurityException("RC_EXTRA_SIGNATURE_BLOCK");
                    continue;
                }
                CodeSigner[] signers = entry.getCodeSigners();
                if (signers == null || signers.length != 1) throw new SecurityException("RC_UNSIGNED_OR_MULTISIGNED_PAYLOAD");
                X509Certificate leaf = (X509Certificate) signers[0].getSignerCertPath().getCertificates().get(0);
                leaf.checkValidity();
                String sha = HexFormat.of().formatHex(MessageDigest.getInstance("SHA-256").digest(leaf.getEncoded()));
                if (!sha.equals(args[1]) || leaf.getSubjectX500Principal().getName().toLowerCase(java.util.Locale.ROOT).contains("android debug"))
                    throw new SecurityException("RC_SIGNER_CERTIFICATE_MISMATCH");
                payload++;
            }
        }
        if (manifests != 1 || signatureFiles.size() != 1 || signatureBlocks.size() != 1 || !signatureFiles.equals(signatureBlocks))
            throw new SecurityException("RC_ORPHAN_OR_EXTRA_SIGNATURE_METADATA");
        if (payload == 0) throw new SecurityException("RC_EMPTY_SIGNED_PAYLOAD");
        System.out.println("{\"verifiedPayloadEntries\":" + payload + ",\"signingFingerprintSha256\":\"" + args[1] + "\"}");
    }
}
