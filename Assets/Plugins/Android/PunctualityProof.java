package com.stp.railway;

import android.app.Activity;
import android.content.ClipData;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Typeface;
import android.net.Uri;
import android.widget.Toast;
import androidx.core.content.FileProvider;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;

/** Draws only the player's displayed proof, with a read grant for this single cached PNG. */
public final class PunctualityProof {
    public static final class Provider extends FileProvider {}
    private PunctualityProof() {}
    public static void share(Activity activity, String title, String route, String grid, String time,
                             String stars, String statement, String chooser) {
        activity.runOnUiThread(() -> {
            Bitmap image = Bitmap.createBitmap(1080, 840, Bitmap.Config.ARGB_8888);
            try {
                Canvas page = new Canvas(image);
                Paint ink = new Paint(Paint.ANTI_ALIAS_FLAG);
                page.drawColor(Color.rgb(242, 230, 201));
                ink.setColor(Color.rgb(15, 42, 87));
                page.drawRect(0, 0, 1080, 180, ink);
                ink.setTypeface(Typeface.create("sans-serif-condensed", Typeface.BOLD));
                ink.setTextSize(60); ink.setColor(Color.WHITE);
                page.drawText(title, 78, 113, ink);
                ink.setColor(Color.rgb(26, 49, 66)); ink.setTextSize(34);
                page.drawText(route + "   ·   " + grid, 80, 252, ink);
                ink.setTextSize(82); page.drawText(time, 78, 374, ink);
                page.save(); page.rotate(-3, 500, 475);
                ink.setColor(Color.rgb(132, 46, 49)); ink.setStyle(Paint.Style.STROKE); ink.setStrokeWidth(7);
                page.drawRoundRect(72, 408, 975, 526, 15, 15, ink);
                ink.setStyle(Paint.Style.FILL); ink.setTextSize(46); page.drawText(stars, 104, 484, ink);
                page.restore(); ink.setColor(Color.rgb(38, 52, 56)); ink.setTextSize(32);
                String line = ""; int baseline = 626;
                for (String word : statement.split(" ")) {
                    if (ink.measureText(line + word) > 910) {
                        page.drawText(line, 80, baseline, ink); baseline += 48; line = "";
                    }
                    line += word + " ";
                }
                page.drawText(line, 80, baseline, ink);
                File dir = new File(activity.getCacheDir(), "punctuality");
                if (!dir.isDirectory() && !dir.mkdirs()) throw new IOException();
                File file = new File(dir, "Puenktlichkeitsnachweis.png");
                try (FileOutputStream stream = new FileOutputStream(file)) {
                    if (!image.compress(Bitmap.CompressFormat.PNG, 100, stream)) throw new IOException();
                }
                Uri uri = FileProvider.getUriForFile(activity, activity.getPackageName() + ".punctuality", file);
                Intent intent = new Intent(Intent.ACTION_SEND).setType("image/png");
                intent.putExtra(Intent.EXTRA_STREAM, uri);
                intent.setClipData(ClipData.newRawUri(title, uri));
                intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
                activity.startActivity(Intent.createChooser(intent, chooser));
            } catch (IOException | RuntimeException error) {
                Toast.makeText(activity, "Der Nachweis konnte nicht geteilt werden.", Toast.LENGTH_SHORT).show();
            } finally { image.recycle(); }
        });
    }
}
