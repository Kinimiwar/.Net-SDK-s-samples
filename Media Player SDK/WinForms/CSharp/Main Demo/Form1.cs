using System.Threading;

namespace Media_Player_Demo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;

    using VisioForge.Controls.UI.WinForms;
    using VisioForge.Tools;
    using VisioForge.Tools.MediaInfo;
    using VisioForge.Types;
    using VisioForge.Types.GPUVideoEffects;
    using VisioForge.Types.VideoEffects;

    public partial class Form1 : Form
    {
        private List<AudioChannelMapperItem> audioChannelMapperItems = new List<AudioChannelMapperItem>();

        // Zoom
        private double zoom = 1.0;

        private int zoomShiftX;

        private int zoomShiftY;

        private readonly MediaInfoReader MediaInfo = new MediaInfoReader();

        private List<Form> multiscreenWindows = new List<Form>();

        public Form1()
        {
            InitializeComponent();
        }

        private void tbLightness_Scroll(object sender, EventArgs e)
        {
            IVFVideoEffectLightness lightness;
            var effect = MediaPlayer1.Video_Effects_Get("Lightness");
            if (effect == null)
            {
                lightness = new VFVideoEffectLightness(true, tbLightness.Value);
                MediaPlayer1.Video_Effects_Add(lightness);
            }
            else
            {
                lightness = effect as IVFVideoEffectLightness;
                if (lightness != null)
                {
                    lightness.Value = tbLightness.Value;
                }
            }
        }

        private void tbSaturation_Scroll(object sender, EventArgs e)
        {
            IVFVideoEffectSaturation saturation;
            var effect = MediaPlayer1.Video_Effects_Get("Saturation");
            if (effect == null)
            {
                saturation = new VFVideoEffectSaturation(tbSaturation.Value);
                MediaPlayer1.Video_Effects_Add(saturation);
            }
            else
            {
                saturation = effect as IVFVideoEffectSaturation;
                if (saturation != null)
                {
                    saturation.Value = tbSaturation.Value;
                }
            }
        }

        private void tbDarkness_Scroll(object sender, EventArgs e)
        {
            IVFVideoEffectDarkness darkness;
            var effect = MediaPlayer1.Video_Effects_Get("Darkness");
            if (effect == null)
            {
                darkness = new VFVideoEffectDarkness(true, tbDarkness.Value);
                MediaPlayer1.Video_Effects_Add(darkness);
            }
            else
            {
                darkness = effect as IVFVideoEffectDarkness;
                if (darkness != null)
                {
                    darkness.Value = tbDarkness.Value;
                }
            }
        }

        private void cbTextLogo_CheckedChanged(object sender, EventArgs e)
        {
            btTextLogoUpdateParams_Click(null, null);
        }

        private void cbGreyscale_CheckedChanged(object sender, EventArgs e)
        {
            IVFVideoEffectGrayscale grayscale;
            var effect = MediaPlayer1.Video_Effects_Get("Grayscale");
            if (effect == null)
            {
                grayscale = new VFVideoEffectGrayscale(cbGreyscale.Checked);
                MediaPlayer1.Video_Effects_Add(grayscale);
            }
            else
            {
                grayscale = effect as IVFVideoEffectGrayscale;
                if (grayscale != null)
                {
                    grayscale.Enabled = cbGreyscale.Checked;
                }
            }
        }

        private void cbInvert_CheckedChanged(object sender, EventArgs e)
        {
            IVFVideoEffectInvert invert;
            var effect = MediaPlayer1.Video_Effects_Get("Invert");
            if (effect == null)
            {
                invert = new VFVideoEffectInvert(cbInvert.Checked);
                MediaPlayer1.Video_Effects_Add(invert);
            }
            else
            {
                invert = effect as IVFVideoEffectInvert;
                if (invert != null)
                {
                    invert.Enabled = cbInvert.Checked;
                }
            }
        }

        private void btFont_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                btTextLogoUpdateParams_Click(null, null);
            }
        }

        private void btTextLogoUpdateParams_Click(object sender, EventArgs e)
        {
            VFTextRotationMode rotate;
            VFTextFlipMode flip;

            StringFormat formatFlags = new StringFormat();

            if (cbTextLogoVertical.Checked)
            {
                formatFlags.FormatFlags |= StringFormatFlags.DirectionVertical;
            }

            if (cbTextLogoRightToLeft.Checked)
            {
                formatFlags.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }

            formatFlags.Alignment = (StringAlignment)cbTextLogoAlign.SelectedIndex;

            IVFVideoEffectTextLogo textLogo;
            var effect = MediaPlayer1.Video_Effects_Get("TextLogo");
            if (effect == null)
            {
                textLogo = new VFVideoEffectTextLogo(cbTextLogo.Checked);
                MediaPlayer1.Video_Effects_Add(textLogo);
            }
            else
            {
                textLogo = effect as IVFVideoEffectTextLogo;
            }

            if (textLogo == null)
            {
                MessageBox.Show("Unable to configure text logo effect.");
                return;
            }

            textLogo.Enabled = cbTextLogo.Checked;
            textLogo.Text = edTextLogo.Text;
            textLogo.Left = Convert.ToInt32(edTextLogoLeft.Text);
            textLogo.Top = Convert.ToInt32(edTextLogoTop.Text);
            textLogo.Font = fontDialog1.Font;
            textLogo.FontColor = fontDialog1.Color;

            textLogo.BackgroundTransparent = cbTextLogoTranspBG.Checked;
            textLogo.BackgroundColor = pnTextLogoBGColor.BackColor;
            textLogo.StringFormat = formatFlags;
            textLogo.Antialiasing = (TextRenderingHint)cbTextLogoAntialiasing.SelectedIndex;
            textLogo.DrawQuality = (InterpolationMode)cbTextLogoDrawMode.SelectedIndex;

            if (cbTextLogoUseRect.Checked)
            {
                textLogo.RectWidth = Convert.ToInt32(edTextLogoWidth.Text);
                textLogo.RectHeight = Convert.ToInt32(edTextLogoHeight.Text);
            }
            else
            {
                textLogo.RectWidth = 0;
                textLogo.RectHeight = 0;
            }

            if (rbTextLogoDegree0.Checked)
            {
                rotate = VFTextRotationMode.RmNone;
            }
            else if (rbTextLogoDegree90.Checked)
            {
                rotate = VFTextRotationMode.Rm90;
            }
            else if (rbTextLogoDegree180.Checked)
            {
                rotate = VFTextRotationMode.Rm180;
            }
            else
            {
                rotate = VFTextRotationMode.Rm270;
            }

            if (rbTextLogoFlipNone.Checked)
            {
                flip = VFTextFlipMode.None;
            }
            else if (rbTextLogoFlipX.Checked)
            {
                flip = VFTextFlipMode.X;
            }
            else if (rbTextLogoFlipY.Checked)
            {
                flip = VFTextFlipMode.Y;
            }
            else
            {
                flip = VFTextFlipMode.XAndY;
            }

            textLogo.RotationMode = rotate;
            textLogo.FlipMode = flip;

            textLogo.GradientEnabled = cbTextLogoGradientEnabled.Checked;
            textLogo.GradientMode = (VFTextGradientMode)cbTextLogoGradMode.SelectedIndex;
            textLogo.GradientColor1 = pnTextLogoGradColor1.BackColor;
            textLogo.GradientColor2 = pnTextLogoGradColor2.BackColor;

            textLogo.BorderMode = (VFTextEffectMode)cbTextLogoEffectrMode.SelectedIndex;
            textLogo.BorderInnerColor = pnTextLogoInnerColor.BackColor;
            textLogo.BorderOuterColor = pnTextLogoOuterColor.BackColor;
            textLogo.BorderInnerSize = Convert.ToInt32(edTextLogoInnerSize.Text);
            textLogo.BorderOuterSize = Convert.ToInt32(edTextLogoOuterSize.Text);

            textLogo.Shape = cbTextLogoShapeEnabled.Checked;
            textLogo.ShapeLeft = Convert.ToInt32(edTextLogoShapeLeft.Text);
            textLogo.ShapeTop = Convert.ToInt32(edTextLogoShapeTop.Text);
            textLogo.ShapeType = (VFTextShapeType)cbTextLogoShapeType.SelectedIndex;
            textLogo.ShapeWidth = Convert.ToInt32(edTextLogoShapeWidth.Text);
            textLogo.ShapeHeight = Convert.ToInt32(edTextLogoShapeHeight.Text);
            textLogo.ShapeColor = pnTextLogoShapeColor.BackColor;

            textLogo.TransparencyLevel = tbTextLogoTransp.Value;

            if (cbTextLogoDateTime.Checked)
            {
                textLogo.Mode = TextLogoMode.DateTime;
                textLogo.DateTimeMask = "yyyy-MM-dd. hh:mm:ss";
            }
            else
            {
                textLogo.Mode = TextLogoMode.Text;
            }

            if (cbTextLogoFadeIn.Checked)
            {
                textLogo.FadeIn = true;
                textLogo.FadeInDuration = 5000;
            }
            else
            {
                textLogo.FadeIn = false;
            }

            if (cbTextLogoFadeOut.Checked)
            {
                textLogo.FadeOut = true;
                textLogo.FadeOutDuration = 5000;
            }
            else
            {
                textLogo.FadeOut = false;
            }

            textLogo.Update();
        }

        private void cbImageLogo_CheckedChanged(object sender, EventArgs e)
        {
            if (!File.Exists(edImageLogoFilename.Text))
            {
                if (cbImageLogo.Checked)
                {
                    MessageBox.Show("Unable to find " + edImageLogoFilename.Text);
                    cbImageLogo.Checked = false;
                }

                return;
            }

            IVFVideoEffectImageLogo imageLogo;
            var effect = MediaPlayer1.Video_Effects_Get("ImageLogo");
            if (effect == null)
            {
                imageLogo = new VFVideoEffectImageLogo(cbImageLogo.Checked);
                MediaPlayer1.Video_Effects_Add(imageLogo);
            }
            else
            {
                imageLogo = effect as IVFVideoEffectImageLogo;
            }

            if (imageLogo == null)
            {
                MessageBox.Show("Unable to configure image logo effect.");
                return;
            }

            imageLogo.Enabled = cbImageLogo.Checked;
            imageLogo.Filename = edImageLogoFilename.Text;
            imageLogo.Left = Convert.ToUInt32(edImageLogoLeft.Text);
            imageLogo.Top = Convert.ToUInt32(edImageLogoTop.Text);
            imageLogo.TransparencyLevel = tbImageLogoTransp.Value;
            imageLogo.ColorKey = pnImageLogoColorKey.ForeColor;
            imageLogo.UseColorKey = cbImageLogoUseColorKey.Checked;
            imageLogo.AnimationEnabled = true;

            if (cbImageLogoShowAlways.Checked)
            {
                imageLogo.StartTime = 0;
                imageLogo.StopTime = 0;
            }
            else
            {
                imageLogo.StartTime = Convert.ToInt32(edImageLogoStartTime.Text);
                imageLogo.StopTime = Convert.ToInt32(edImageLogoStopTime.Text);
            }
        }

        private void btSelectImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                this.edImageLogoFilename.Text = openFileDialog2.FileName;
            }
        }

        private void tbGraphicLogoTransp_Scroll(object sender, EventArgs e)
        {
            this.cbImageLogo_CheckedChanged(null, null);
        }

        private void cbGraphicLogoUseColorKey_CheckedChanged(object sender, EventArgs e)
        {
            this.cbImageLogo_CheckedChanged(null, null);
        }

        private void pnGraphicLogoColorKey_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = this.pnImageLogoColorKey.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.pnImageLogoColorKey.BackColor = colorDialog1.Color;
            }
        }

        private void cbGraphicLogoShowAlways_CheckedChanged(object sender, EventArgs e)
        {
            this.edImageLogoStartTime.Enabled = !this.cbImageLogoShowAlways.Checked;
            this.edImageLogoStopTime.Enabled = !this.cbImageLogoShowAlways.Checked;
            lbGraphicLogoStartTime.Enabled = !this.cbImageLogoShowAlways.Checked;
            lbGraphicLogoStopTime.Enabled = !this.cbImageLogoShowAlways.Checked;

            this.cbImageLogo_CheckedChanged(null, null);
        }

        private void btOSDInit_Click(object sender, EventArgs e)
        {
            MediaPlayer1.OSD_Init();
        }

        private void btOSDDeinit_Click(object sender, EventArgs e)
        {
            MediaPlayer1.OSD_Destroy();
        }

        private void btOSDClearLayers_Click(object sender, EventArgs e)
        {
            MediaPlayer1.OSD_Layers_Clear();
            lbOSDLayers.Items.Clear();
        }

        private void btOSDLayerAdd_Click(object sender, EventArgs e)
        {
            MediaPlayer1.OSD_Layers_Create(
                Convert.ToInt32(edOSDLayerLeft.Text),
                Convert.ToInt32(edOSDLayerTop.Text),
                Convert.ToInt32(edOSDLayerWidth.Text),
                Convert.ToInt32(edOSDLayerHeight.Text));
            lbOSDLayers.Items.Add("layer " + Convert.ToString(lbOSDLayers.Items.Count + 1));
        }

        private void btOSDApplyLayer_Click(object sender, EventArgs e)
        {
            if (lbOSDLayers.SelectedIndex != -1)
            {
                MediaPlayer1.OSD_Layers_Apply(lbOSDLayers.SelectedIndex);
            }
        }

        private void btOSDSelectImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                edOSDImageFilename.Text = openFileDialog2.FileName;
            }
        }

        private void pnOSDColorKey_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pnOSDColorKey.BackColor = colorDialog1.Color;
            }
        }

        private void btOSDImageDraw_Click(object sender, EventArgs e)
        {
            if (lbOSDLayers.SelectedIndex != -1)
            {
                if (cbOSDImageTranspColor.Checked)
                {
                    MediaPlayer1.OSD_Layers_Draw_ImageFromFile(
                        lbOSDLayers.SelectedIndex,
                        edOSDImageFilename.Text,
                        Convert.ToInt32(edOSDImageLeft.Text),
                        Convert.ToInt32(edOSDImageTop.Text),
                        true,
                        pnOSDColorKey.BackColor);
                }
                else
                {
                    MediaPlayer1.OSD_Layers_Draw_ImageFromFile(
                        lbOSDLayers.SelectedIndex,
                        edOSDImageFilename.Text,
                        Convert.ToInt32(edOSDImageLeft.Text),
                        Convert.ToInt32(edOSDImageTop.Text),
                        false,
                        Color.Empty);
                }
            }
        }

        private void btOSDSelectFont_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                edOSDText.Font = fontDialog1.Font;
                edOSDText.ForeColor = fontDialog1.Color;
            }
        }

        private void btOSDTextDraw_Click(object sender, EventArgs e)
        {
            if (lbOSDLayers.SelectedIndex != -1)
            {
                Font fnt = edOSDText.Font;
                Color color = edOSDText.ForeColor;

                MediaPlayer1.OSD_Layers_Draw_Text(
                    lbOSDLayers.SelectedIndex,
                    Convert.ToInt32(edOSDTextLeft.Text),
                    Convert.ToInt32(edOSDTextTop.Text),
                    edOSDText.Text,
                    fnt,
                    color);
            }
        }

        private void btOSDSetTransp_Click(object sender, EventArgs e)
        {
            if (lbOSDLayers.SelectedIndex != -1)
            {
                MediaPlayer1.OSD_Layers_SetTransparency(lbOSDLayers.SelectedIndex, (byte)tbOSDTranspLevel.Value);
                MediaPlayer1.OSD_Layers_Apply(lbOSDLayers.SelectedIndex);
            }
        }

        private void btSelectScreenshotsFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                edScreenshotsFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btSaveScreenshot_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;

            string s = dt.Hour + "_" + dt.Minute + "_" + dt.Second + "_" + dt.Millisecond;

            int customWidth = 0;
            int customHeight = 0;

            if (cbScreenshotResize.Checked)
            {
                customWidth = Convert.ToInt32(edScreenshotWidth.Text);
                customHeight = Convert.ToInt32(edScreenshotHeight.Text);
            }

            switch (cbImageType.SelectedIndex)
            {
                case 0:
                    MediaPlayer1.Frame_Save(edScreenshotsFolder.Text + "\\" + s + ".bmp", VFImageFormat.BMP, 0, customWidth, customHeight);
                    break;
                case 1:
                    MediaPlayer1.Frame_Save(edScreenshotsFolder.Text + "\\" + s + ".jpg", VFImageFormat.JPEG, tbJPEGQuality.Value, customWidth, customHeight);
                    break;
                case 2:
                    MediaPlayer1.Frame_Save(edScreenshotsFolder.Text + "\\" + s + ".gif", VFImageFormat.GIF, 0, customWidth, customHeight);
                    break;
                case 3:
                    MediaPlayer1.Frame_Save(edScreenshotsFolder.Text + "\\" + s + ".png", VFImageFormat.PNG, 0, customWidth, customHeight);
                    break;
                case 4:
                    MediaPlayer1.Frame_Save(edScreenshotsFolder.Text + "\\" + s + ".tiff", VFImageFormat.TIFF, 0, customWidth, customHeight);
                    break;
            }
        }

        private void lbDVDTitles_Click(object sender, EventArgs e)
        {
            if (lbDVDTitles.SelectedIndex != -1)
            {
                cbDVDAudio.Items.Clear();
                cbDVDSubtitles.Items.Clear();

                MediaInfo.DVD_Fill_Title_Info(lbDVDTitles.SelectedIndex);

                string s = MediaInfo.DVD_Title_MainAttributes_VideoAttributes_Compression + " ";
                s = s + MediaInfo.DVD_Title_MainAttributes_VideoAttributes_SourceResolutionX + "x" + MediaInfo.DVD_Title_MainAttributes_VideoAttributes_SourceResolutionY + " ";
                s = s + MediaInfo.DVD_Title_MainAttributes_VideoAttributes_AspectX + ":" + MediaInfo.DVD_Title_MainAttributes_VideoAttributes_AspectY + " ";

                edDVDVideo.Text = s;

                for (int i = 0; i < MediaInfo.DVD_Title_MainAttributes_NumberOfAudioStreams; i++)
                {
                    MediaInfo.DVD_Fill_Title_Audio_Info(lbDVDTitles.SelectedIndex, i);
                    s = MediaInfo.DVD_Title_MainAttributes_AudioAttributes_AudioFormat;

                    s = s + " - ";
                    s = s + MediaInfo.DVD_Title_MainAttributes_AudioAttributes_NumberOfChannels + "ch" + " - ";
                    s = s + MediaInfo.DVD_Title_MainAttributes_AudioAttributes_LanguageS;

                    cbDVDAudio.Items.Add(s);
                }

                if (cbDVDAudio.Items.Count > 0)
                {
                    cbDVDAudio.SelectedIndex = 0;
                }

                for (int i = 0; i < MediaInfo.DVD_Title_MainAttributes_NumberOfSubpictureStreams; i++)
                {
                    MediaInfo.DVD_Fill_Title_Subpicture_Info(lbDVDTitles.SelectedIndex, i);
                    cbDVDSubtitles.Items.Add(MediaInfo.DVD_Title_MainAttributes_SubpictureAttributes_LanguageS);
                }

                if (cbDVDSubtitles.Items.Count > 0)
                {
                    cbDVDSubtitles.SelectedIndex = 0;
                }
            }
        }

        private void tbBalance1_Scroll(object sender, EventArgs e)
        {
            if (cbAudioStream1.Checked || MediaPlayer1.Audio_Streams_AllInOne())
            {
                MediaPlayer1.Audio_OutputDevice_Balance_Set(0, tbBalance1.Value);
            }
        }

        private void tbBalance2_Scroll(object sender, EventArgs e)
        {
            if (cbAudioStream2.Checked)
            {
                MediaPlayer1.Audio_OutputDevice_Balance_Set(1, tbBalance2.Value);
            }
        }

        private void tbBalance3_Scroll(object sender, EventArgs e)
        {
            if (cbAudioStream3.Checked)
            {
                MediaPlayer1.Audio_OutputDevice_Balance_Set(2, tbBalance3.Value);
            }
        }

        private void tbBalance4_Scroll(object sender, EventArgs e)
        {
            if (cbAudioStream4.Checked)
            {
                MediaPlayer1.Audio_OutputDevice_Balance_Set(3, tbBalance4.Value);
            }
        }

        private void tbSpeed_Scroll(object sender, EventArgs e)
        {
            if (MediaPlayer1.Source_Mode != VFMediaPlayerSource.DVD_DS)
            {
                MediaPlayer1.SetSpeed(tbSpeed.Value / 10.0);
            }
            else
            {
                MediaPlayer1.DVD_SetSpeed(tbSpeed.Value / 10.0, false);
            }
        }

        private void tbVolume1_Scroll(object sender, EventArgs e)
        {
            if (cbAudioStream1.Checked || MediaPlayer1.Audio_Streams_AllInOne())
            {
                MediaPlayer1.Audio_OutputDevice_Volume_Set(0, tbVolume1.Value);
            }
        }

        private void tbTimeline_Scroll(object sender, EventArgs e)
        {
            if (Convert.ToInt32(timer1.Tag) == 0)
            {
                MediaPlayer1.Position_Set_Time(tbTimeline.Value * 1000);
            }
        }

        private void tbVolume2_Scroll(object sender, EventArgs e)
        {
            if (cbAudioStream2.Checked)
            {
                MediaPlayer1.Audio_OutputDevice_Volume_Set(1, tbVolume2.Value);
            }
        }

        private void tbVolume3_Scroll(object sender, EventArgs e)
        {
            if (cbAudioStream3.Checked)
            {
                MediaPlayer1.Audio_OutputDevice_Volume_Set(2, tbVolume3.Value);
            }
        }

        private void tbVolume4_Scroll(object sender, EventArgs e)
        {
            if (cbAudioStream4.Checked)
            {
                MediaPlayer1.Audio_OutputDevice_Volume_Set(3, tbVolume4.Value);
            }
        }

        private void btReadInfo_Click(object sender, EventArgs e)
        {
            mmInfo.Clear();

            // clear audio controls
            cbAudioStream1.Enabled = false;
            cbAudioStream2.Enabled = false;
            cbAudioStream3.Enabled = false;
            cbAudioStream4.Enabled = false;
            tbVolume1.Enabled = false;
            tbVolume2.Enabled = false;
            tbVolume3.Enabled = false;
            tbVolume4.Enabled = false;
            tbBalance1.Enabled = false;
            tbBalance2.Enabled = false;
            tbBalance3.Enabled = false;
            tbBalance4.Enabled = false;

            MediaInfo.Filename = edFilenameOrURL.Text;

            switch (cbSourceMode.SelectedIndex)
            {
                case 0:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.LAV;
                    break;
                case 1:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.File_FFMPEG;
                    break;
                case 2:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.File_DS;
                    break;
                case 3:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.File_VLC;
                    break;
                case 4:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.DVD_DS;
                    break;
                case 5:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.BluRay;
                    break;
                case 6:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.Memory_DS;
                    break;
                case 7:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.Memory_FFMPEG;
                    break;
                case 8:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.MMS_WMV_DS;
                    break;
                case 9:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.HTTP_RTSP_FFMPEG;
                    break;
                case 10:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.HTTP_RTSP_VLC;
                    break;
                case 11:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.Encrypted_File_DS;
                    break;
                case 12:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.CustomSource;

                    if (!string.IsNullOrEmpty(edCustomSourceFilter.Text))
                    {
                        MediaPlayer1.Source_Custom_CLSID = edCustomSourceFilter.Text.Trim();
                    }

                    break;
            }

            if ((MediaPlayer1.Source_Mode == VFMediaPlayerSource.File_DS) ||
                (MediaPlayer1.Source_Mode == VFMediaPlayerSource.File_FFMPEG) ||
                (MediaPlayer1.Source_Mode == VFMediaPlayerSource.LAV) ||
                (MediaPlayer1.Source_Mode == VFMediaPlayerSource.Encrypted_File_DS))
            {
                // "Read info" button
                if (sender != null)
                {
                    VFFilePlaybackError ErrorCode;
                    string ErrorText;

                    // ReSharper disable once AccessToStaticMemberViaDerivedType
                    if (MediaInfoReader.IsFilePlayable(edFilenameOrURL.Text, out ErrorCode, out ErrorText))
                    {
                        mmInfo.Text += "This file is playable" + Environment.NewLine;
                    }
                    else
                    {
                        mmInfo.Text += "This file is not playable" + Environment.NewLine;
                    }
                }

                VFEncryptionKeyType keyType;
                object key;
                if (rbEncryptionKeyString.Checked)
                {
                    keyType = VFEncryptionKeyType.String;
                    key = edEncryptionKeyString.Text;
                }
                else if (rbEncryptionKeyFile.Checked)
                {
                    keyType = VFEncryptionKeyType.File;
                    key = edEncryptionKeyFile.Text;
                }
                else
                {
                    keyType = VFEncryptionKeyType.Binary;
                    key = MediaPlayer.ConvertHexStringToByteArray(edEncryptionKeyHEX.Text);
                }

                MediaInfo.ReadFileInfo(MediaPlayer1.Source_Mode == VFMediaPlayerSource.Encrypted_File_DS, key, keyType, cbUseLibMediaInfo.Checked);

                for (int i = 0; i < MediaInfo.Video_Streams_Count(); i++)
                {
                    mmInfo.Text += string.Empty + Environment.NewLine;
                    mmInfo.Text += "Video #" + Convert.ToString(i + 1) + Environment.NewLine;
                    mmInfo.Text += "Codec: " + MediaInfo.Video_Codec(i) + Environment.NewLine;
                    mmInfo.Text += "Duration: " + MediaInfo.Video_Duration(i) + Environment.NewLine;
                    mmInfo.Text += "Width: " + MediaInfo.Video_Width(i) + Environment.NewLine;
                    mmInfo.Text += "Height: " + MediaInfo.Video_Height(i) + Environment.NewLine;
                    mmInfo.Text += "FOURCC: " + MediaInfo.Video_FourCC(i) + Environment.NewLine;
                    mmInfo.Text += "Aspect Ratio: " + MediaInfo.Video_AspectRatioStr(i) + Environment.NewLine;
                    mmInfo.Text += "Frame rate: " + MediaInfo.Video_FrameRate(i) + Environment.NewLine;
                    mmInfo.Text += "Bitrate: " + MediaInfo.Video_Bitrate(i) + Environment.NewLine;
                    mmInfo.Text += "Frames count: " + MediaInfo.Video_FramesCount(i) + Environment.NewLine;
                }

                for (int i = 0; i < MediaInfo.Audio_Streams_Count(); i++)
                {
                    mmInfo.Text += string.Empty + Environment.NewLine;
                    mmInfo.Text += "Audio #" + Convert.ToString(i + 1) + Environment.NewLine;
                    mmInfo.Text += "Codec: " + MediaInfo.Audio_Codec(i) + Environment.NewLine;
                    mmInfo.Text += "Codec info: " + MediaInfo.Audio_CodecInfo(i) + Environment.NewLine;
                    mmInfo.Text += "Duration: " + MediaInfo.Audio_Duration(i) + Environment.NewLine;
                    mmInfo.Text += "Bitrate: " + MediaInfo.Audio_Bitrate(i) + Environment.NewLine;
                    mmInfo.Text += "Channels: " + MediaInfo.Audio_Channels(i) + Environment.NewLine;
                    mmInfo.Text += "Sample rate: " + MediaInfo.Audio_SampleRate(i) + Environment.NewLine;
                    mmInfo.Text += "BPS: " + MediaInfo.Audio_BPS(i) + Environment.NewLine;
                }

                for (int i = 0; i < MediaInfo.Text_Streams_Count(); i++)
                {
                    mmInfo.Text += string.Empty + Environment.NewLine;
                    mmInfo.Text += "Text #" + Convert.ToString(i + 1) + Environment.NewLine;
                    mmInfo.Text += "Codec: " + MediaInfo.Text_Codec(i) + Environment.NewLine;
                    mmInfo.Text += "Name: " + MediaInfo.Text_Name(i) + Environment.NewLine;
                    mmInfo.Text += "Language: " + MediaInfo.Text_Language(i) + Environment.NewLine;
                }

                // timeline
                if (MediaInfo.Video_Streams_Count() > 0)
                {
                    tbTimeline.Maximum = (int)(MediaInfo.Video_DurationMSec(0) / 1000);
                }
                else if (MediaInfo.Audio_Streams_Count() > 0)
                {
                    tbTimeline.Maximum = (int)(MediaInfo.Audio_DurationMSec(0) / 1000);
                }

                // set audio streams tab
                int count = MediaInfo.Audio_Streams_Count();
                bool one_output = MediaInfo.Audio_Streams_AllInOne;

                if (count == 0)
                {
                    return;
                }

                cbAudioStream1.Enabled = true;
                cbAudioStream1.Checked = true;
                tbVolume1.Enabled = true;
                tbBalance1.Enabled = true;

                if (count > 1)
                {
                    cbAudioStream2.Enabled = true;
                    cbAudioStream2.Checked = false;
                    tbVolume2.Enabled = !one_output;
                    tbBalance2.Enabled = !one_output;
                }

                if (count > 2)
                {
                    cbAudioStream3.Enabled = true;
                    cbAudioStream3.Checked = false;
                    tbVolume3.Enabled = !one_output;
                    tbBalance3.Enabled = !one_output;
                }

                if (count > 3)
                {
                    cbAudioStream4.Enabled = true;
                    cbAudioStream4.Checked = false;
                    tbVolume4.Enabled = !one_output;
                    tbBalance4.Enabled = !one_output;
                }

                // additional audio streams added from other audio files
                int count2 = MediaPlayer1.Audio_AdditionalStreams_GetCount();

                if (count2 == 0)
                {
                    return;
                }

                int count3 = count2;

                if ((count2 + count >= 4) && (count3 > 0))
                {
                    cbAudioStream4.Enabled = true;
                    cbAudioStream4.Checked = true;
                    tbVolume4.Enabled = true;
                    tbBalance4.Enabled = true;
                    count3 = count3 - 1;
                }

                if ((count2 + count >= 3) && (count3 > 0))
                {
                    cbAudioStream3.Enabled = true;
                    cbAudioStream3.Checked = true;
                    tbVolume3.Enabled = true;
                    tbBalance3.Enabled = true;
                    count3 = count3 - 1;
                }

                if ((count2 + count >= 2) && (count3 > 0))
                {
                    cbAudioStream2.Enabled = true;
                    cbAudioStream2.Checked = true;
                    tbVolume2.Enabled = true;
                    tbBalance2.Enabled = true;
                    count3 = count3 - 1;
                }

                if ((count2 + count >= 1) && (count3 > 0))
                {
                    cbAudioStream1.Enabled = true;
                    cbAudioStream1.Checked = true;
                    tbVolume1.Enabled = true;
                    tbBalance1.Enabled = true;
                }
            }
            else if (MediaPlayer1.Source_Mode == VFMediaPlayerSource.DVD_DS)
            {
                cbAudioStream1.Enabled = true;
                cbAudioStream1.Checked = true;
                tbVolume1.Enabled = true;
                tbBalance1.Enabled = true;

                edDVDVideo.Text = string.Empty;
                lbDVDTitles.Items.Clear();
                cbDVDAudio.Items.Clear();
                cbDVDSubtitles.Items.Clear();

                cbDVDControlTitle.Items.Clear();
                cbDVDControlChapter.Items.Clear();
                cbDVDControlAudio.Items.Clear();
                cbDVDControlSubtitles.Items.Clear();

                MediaInfo.ReadDVDInfo();

                for (int i = 0; i < MediaInfo.DVD_Disc_NumOfTitles; i++)
                {
                    lbDVDTitles.Items.Add("Title " + Convert.ToString(i + 1));
                    cbDVDControlTitle.Items.Add("Title " + Convert.ToString(i + 1));
                }
            }
            else
            {
                cbAudioStream1.Enabled = true;
                cbAudioStream1.Checked = true;
                tbVolume1.Enabled = true;
                tbBalance1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Tag = 1;
            tbTimeline.Maximum = (int)(MediaPlayer1.Duration_Time() / 1000.0);

            int value = (int)(MediaPlayer1.Position_Get_Time() / 1000.0);
            if ((value > 0) && (value < tbTimeline.Maximum))
            {
                tbTimeline.Value = value;
            }

            lbTime.Text = MediaPlayer.Helpful_SecondsToTimeFormatted(tbTimeline.Value) + "/" + MediaPlayer.Helpful_SecondsToTimeFormatted(tbTimeline.Maximum);

            if (MediaPlayer1.Source_Mode == VFMediaPlayerSource.DVD_DS)
            {
                if (MediaPlayer1.DVD_Chapter_GetCurrent() != cbDVDControlChapter.SelectedIndex && cbDVDControlChapter.SelectedIndex != -1)
                {
                    cbDVDControlChapter.SelectedIndex = MediaPlayer1.DVD_Chapter_GetCurrent();
                }
            }

            timer1.Tag = 0;
        }

        private void FillAdjustRanges()
        {
            // updating adjust settings
            float max;
            float defaultValue;
            float min;
            float step;

            if (MediaPlayer1.Video_Renderer_VideoAdjust_GetRanges(
                VFVideoRendererAdjustment.Brightness, out min, out max, out step, out defaultValue))
            {
                tbAdjBrightness.Minimum = (int)min * 10;
                tbAdjBrightness.Maximum = (int)max * 10;
                tbAdjBrightness.SmallChange = (int)step * 10;

                int def = (int)defaultValue * 10;

                if (def > (int)max * 10)
                {
                    def = (int)max * 10;
                }

                if (def < (int)min * 10)
                {
                    def = (int)min * 10;
                }

                tbAdjBrightness.Value = def;
                lbAdjBrightnessMin.Text = "Min: " + Convert.ToString(min, CultureInfo.InvariantCulture);
                lbAdjBrightnessMax.Text = "Max: " + Convert.ToString(max, CultureInfo.InvariantCulture);
                lbAdjBrightnessCurrent.Text = "Current: " + Convert.ToString(defaultValue, CultureInfo.InvariantCulture);
            }

            if (MediaPlayer1.Video_Renderer_VideoAdjust_GetRanges(
                VFVideoRendererAdjustment.Hue, out min, out max, out step, out defaultValue))
            {
                tbAdjHue.Minimum = (int)min * 10;
                tbAdjHue.Maximum = (int)max * 10;
                tbAdjHue.SmallChange = (int)step * 10;

                int def = (int)defaultValue * 10;

                if (def > (int)max * 10)
                {
                    def = (int)max * 10;
                }

                if (def < (int)min * 10)
                {
                    def = (int)min * 10;
                }

                tbAdjHue.Value = def;
                lbAdjHueMin.Text = "Min: " + Convert.ToString(min, CultureInfo.InvariantCulture);
                lbAdjHueMax.Text = "Max: " + Convert.ToString(max, CultureInfo.InvariantCulture);
                lbAdjHueCurrent.Text = "Current: " + Convert.ToString(defaultValue, CultureInfo.InvariantCulture);
            }

            if (MediaPlayer1.Video_Renderer_VideoAdjust_GetRanges(
                VFVideoRendererAdjustment.Saturation, out min, out max, out step, out defaultValue))
            {
                tbAdjSaturation.Minimum = (int)min * 10;
                tbAdjSaturation.Maximum = (int)max * 10;
                tbAdjSaturation.SmallChange = (int)step * 10;

                int def = (int)defaultValue * 10;

                if (def > (int)max * 10)
                {
                    def = (int)max * 10;
                }

                if (def < (int)min * 10)
                {
                    def = (int)min * 10;
                }

                tbAdjSaturation.Value = def;
                lbAdjSaturationMin.Text = "Min: " + Convert.ToString(min, CultureInfo.InvariantCulture);
                lbAdjSaturationMax.Text = "Max: " + Convert.ToString(max, CultureInfo.InvariantCulture);
                lbAdjSaturationCurrent.Text = "Current: " + Convert.ToString(defaultValue, CultureInfo.InvariantCulture);
            }

            if (MediaPlayer1.Video_Renderer_VideoAdjust_GetRanges(
                VFVideoRendererAdjustment.Contrast, out min, out max, out step, out defaultValue))
            {
                tbAdjContrast.Minimum = (int)min * 10;
                tbAdjContrast.Maximum = (int)max * 10;
                tbAdjContrast.SmallChange = (int)step * 10;

                int def = (int)defaultValue * 10;

                if (def > (int)max * 10)
                {
                    def = (int)max * 10;
                }

                if (def < (int)min * 10)
                {
                    def = (int)min * 10;
                }

                tbAdjContrast.Value = def;
                lbAdjContrastMin.Text = "Min: " + Convert.ToString(min, CultureInfo.InvariantCulture);
                lbAdjContrastMax.Text = "Max: " + Convert.ToString(max, CultureInfo.InvariantCulture);
                lbAdjContrastCurrent.Text = "Current: " + Convert.ToString(defaultValue, CultureInfo.InvariantCulture);
            }
        }

        private void ConfigureChromaKey()
        {
            if (cbChromaKeyEnabled.Checked)
            {
                MediaPlayer1.ChromaKey = new ChromaKeySettings
                {
                    ContrastHigh = this.tbChromaKeyContrastHigh.Value,
                    ContrastLow = this.tbChromaKeyContrastLow.Value,
                    ImageFilename = this.edChromaKeyImage.Text
                };

                if (rbChromaKeyGreen.Checked)
                {
                    MediaPlayer1.ChromaKey.Color = VFChromaColor.Green;
                }
                else if (rbChromaKeyBlue.Checked)
                {
                    MediaPlayer1.ChromaKey.Color = VFChromaColor.Blue;
                }
                else
                {
                    MediaPlayer1.ChromaKey.Color = VFChromaColor.Red;
                }
            }
            else
            {
                MediaPlayer1.ChromaKey = null;
            }
        }

        static void ShowOnScreen(Form window, int screenNumber)
        {
            if (screenNumber >= 0 && screenNumber < Screen.AllScreens.Length)
            {
                window.Location = Screen.AllScreens[screenNumber].WorkingArea.Location;

                window.Show();

                window.Width = Screen.AllScreens[screenNumber].Bounds.Width;
                window.Height = Screen.AllScreens[screenNumber].Bounds.Height;
                window.Left = Screen.AllScreens[screenNumber].Bounds.Left;
                window.Top = Screen.AllScreens[screenNumber].Bounds.Top;
                window.TopMost = true;
                window.FormBorderStyle = FormBorderStyle.None;
                window.WindowState = FormWindowState.Maximized;
            }
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Debug_Mode = cbDebugMode.Checked;
            
            zoom = 1.0;
            zoomShiftX = 0;
            zoomShiftY = 0;

            mmLog.Clear();

            MediaPlayer1.Video_Renderer.Zoom_Ratio = 0;
            MediaPlayer1.Video_Renderer.Zoom_ShiftX = 0;
            MediaPlayer1.Video_Renderer.Zoom_ShiftY = 0;

            MediaPlayer1.Info_UseLibMediaInfo = cbUseLibMediaInfo.Checked;

            if (rbVideoDecoderDefault.Checked)
            {
                MediaPlayer1.Custom_Video_Decoder = string.Empty;
            }
            else if (rbVideoDecoderFFDShow.Checked)
            {
                MediaPlayer1.Custom_Video_Decoder = "ffdshow Video Decoder";
            }
            else if (rbVideoDecoderMS.Checked)
            {
                MediaPlayer1.Custom_Video_Decoder = "Microsoft DTV-DVD Video Decoder";
            }
            else if (rbVideoDecoderVFH264.Checked)
            {
                MediaPlayer1.Custom_Video_Decoder = "VisioForge H264 Decoder";
            }
            else if (rbVideoDecoderCustom.Checked)
            {
                MediaPlayer1.Custom_Video_Decoder = cbCustomVideoDecoder.Text;
            }

            if (rbSplitterCustom.Checked)
            {
                MediaPlayer1.Custom_Splitter = cbCustomSplitter.Text;
            }
            else
            {
                MediaPlayer1.Custom_Splitter = string.Empty;
            }

            if (rbAudioDecoderDefault.Checked)
            {
                MediaPlayer1.Custom_Audio_Decoder = string.Empty;
            }
            else if (rbAudioDecoderCustom.Checked)
            {
                MediaPlayer1.Custom_Audio_Decoder = cbCustomAudioDecoder.Text;
            }

            if (lbSourceFiles.Items.Count == 0)
            {
                MessageBox.Show("Playlist is empty!");
                return;
            }

            foreach (var item in lbSourceFiles.Items)
            {
                MediaPlayer1.FilenamesOrURL.Add(item.ToString());
            }

            MediaPlayer1.Loop = cbLoop.Checked;
            MediaPlayer1.Audio_PlayAudio = cbPlayAudio.Checked;

            MediaPlayer1.Video_Renderer.Aspect_Ratio_X = Convert.ToInt32(edAspectRatioX.Text);
            MediaPlayer1.Video_Renderer.Aspect_Ratio_Y = Convert.ToInt32(edAspectRatioY.Text);
            MediaPlayer1.Video_Renderer.Aspect_Ratio_Override = cbAspectRatioUseCustom.Checked;

            switch (cbSourceMode.SelectedIndex)
            {
                case 0:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.LAV;
                    break;
                case 1:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.File_FFMPEG;
                    break;
                case 2:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.File_DS;
                    break;
                case 3:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.File_VLC;
                    break;
                case 4:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.DVD_DS;
                    break;
                case 5:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.BluRay;
                    break;
                case 6:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.Memory_DS;
                    LoadToMemory();
                    break;
                case 7:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.Memory_FFMPEG;
                    LoadToMemory();
                    break;
                case 8:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.MMS_WMV_DS;
                    break;
                case 9:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.HTTP_RTSP_FFMPEG;
                    break;
                case 10:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.HTTP_RTSP_VLC;
                    break;
                case 11:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.Encrypted_File_DS;
                    break;
                case 12:
                    MediaPlayer1.Source_Mode = VFMediaPlayerSource.CustomSource;

                    if (!string.IsNullOrEmpty(edCustomSourceFilter.Text))
                    {
                        MediaPlayer1.Source_Custom_CLSID = edCustomSourceFilter.Text.Trim();
                    }

                    break;
            }

            btReadInfo_Click(null, null);

            MediaPlayer1.Audio_OutputDevice = cbAudioOutputDevice.Text;

            // VU meters
            MediaPlayer1.Audio_VUMeter_Pro_Enabled = cbVUMeterPro.Checked;

            if (MediaPlayer1.Audio_VUMeter_Pro_Enabled)
            {
                MediaPlayer1.Audio_VUMeter_Pro_Volume = tbVUMeterAmplification.Value;

                volumeMeter1.Boost = tbVUMeterBoost.Value / 10.0F;
                volumeMeter2.Boost = tbVUMeterBoost.Value / 10.0F;

                waveformPainter1.Boost = tbVUMeterBoost.Value / 10.0F;
                waveformPainter2.Boost = tbVUMeterBoost.Value / 10.0F;
            }

            if (rbVR.Checked)
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.VideoRenderer;
            }
            else if (rbVMR9.Checked)
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.VMR9;
            }
            else if (rbEVR.Checked)
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.EVR;
            }
            else if (rbDirect2D.Checked)
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.Direct2D;
            }
            else if (rbMadVR.Checked)
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.MadVR;
            }
            else
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.None;
            }

            MediaPlayer1.Video_Renderer.RotationAngle = Convert.ToInt32(cbDirect2DRotate.Text);
            MediaPlayer1.Video_Renderer.BackgroundColor = pnVideoRendererBGColor.BackColor;
            MediaPlayer1.Video_Renderer.Flip_Horizontal = cbScreenFlipHorizontal.Checked;
            MediaPlayer1.Video_Renderer.Flip_Vertical = cbScreenFlipVertical.Checked;

            // Chroma-key
            ConfigureChromaKey();

            // Audio enhancement
            MediaPlayer1.Audio_Enhancer_Enabled = cbAudioEnhancementEnabled.Checked;
            if (MediaPlayer1.Audio_Enhancer_Enabled)
            {
                MediaPlayer1.Audio_Enhancer_Normalize(-1, cbAudioNormalize.Checked);
                MediaPlayer1.Audio_Enhancer_AutoGain(-1, cbAudioAutoGain.Checked);

                ApplyAudioInputGains();
                ApplyAudioOutputGains();

                MediaPlayer1.Audio_Enhancer_Timeshift(-1, tbAudioTimeshift.Value);
            }

            // Audio channels mapping
            if (cbAudioChannelMapperEnabled.Checked)
            {
                MediaPlayer1.Audio_Channel_Mapper = new AudioChannelMapperSettings
                {
                    Routes = this.audioChannelMapperItems.ToArray(),
                    OutputChannelsCount =
                                                                Convert.ToInt32(
                                                                    this.edAudioChannelMapperOutputChannels.Text)
                };
            }
            else
            {
                MediaPlayer1.Audio_Channel_Mapper = null;
            }

            // Audio processing
            MediaPlayer1.Audio_Effects_Clear(-1);
            MediaPlayer1.Audio_Effects_Enabled = cbAudioEffectsEnabled.Checked;
            if (MediaPlayer1.Audio_Effects_Enabled)
            {
                MediaPlayer1.Audio_Effects_Add(-1, VFAudioEffectType.Amplify, cbAudAmplifyEnabled.Checked, -1, -1);
                MediaPlayer1.Audio_Effects_Add(-1, VFAudioEffectType.Equalizer, cbAudEqualizerEnabled.Checked, -1, -1);
                MediaPlayer1.Audio_Effects_Add(-1, VFAudioEffectType.DynamicAmplify, cbAudDynamicAmplifyEnabled.Checked, -1, -1);
                MediaPlayer1.Audio_Effects_Add(-1, VFAudioEffectType.Sound3D, cbAudSound3DEnabled.Checked, -1, -1);
                MediaPlayer1.Audio_Effects_Add(-1, VFAudioEffectType.TrueBass, cbAudTrueBassEnabled.Checked, -1, -1);
                MediaPlayer1.Audio_Effects_Add(-1, VFAudioEffectType.PitchShift, cbAudPitchShiftEnabled.Checked, -1, -1);
            }

            if (cbAudPitchShiftEnabled.Checked)
            {
                tbAudPitchShift_Scroll(null, null);
            }

            // Multiscreen
            MediaPlayer1.MultiScreen_Clear();
            MediaPlayer1.MultiScreen_Enabled = cbMultiscreenDrawOnPanels.Checked || cbMultiscreenDrawOnExternalDisplays.Checked;

            if (cbMultiscreenDrawOnPanels.Checked)
            {
                MediaPlayer1.MultiScreen_AddScreen(pnScreen1.Handle, pnScreen1.Width, pnScreen1.Height);
                MediaPlayer1.MultiScreen_AddScreen(pnScreen2.Handle, pnScreen2.Width, pnScreen2.Height);
            }

            if (cbMultiscreenDrawOnExternalDisplays.Checked)
            {
                if (Screen.AllScreens.Length > 1)
                {
                    for (int i = 1; i < Screen.AllScreens.Length; i++)
                    {
                        var additinalWindow1 = new Form();
                        ShowOnScreen(additinalWindow1, i);
                        MediaPlayer1.MultiScreen_AddScreen(additinalWindow1.Handle, additinalWindow1.Width, additinalWindow1.Height);
                        multiscreenWindows.Add(additinalWindow1);
                    }
                }
            }
        

            // Video effects
            MediaPlayer1.Video_Effects_Enabled = cbEffects.Checked;
            MediaPlayer1.Video_Effects_Clear();

            // Deinterlace
            if (cbDeinterlace.Checked)
            {
                MediaPlayer1.Video_Effects_Enabled = true;
                if (rbDeintBlendEnabled.Checked)
                {
                    IVFVideoEffectDeinterlaceBlend blend;
                    var effect = MediaPlayer1.Video_Effects_Get("DeinterlaceBlend");
                    if (effect == null)
                    {
                        blend = new VFVideoEffectDeinterlaceBlend(true);
                        MediaPlayer1.Video_Effects_Add(blend);
                    }
                    else
                    {
                        blend = effect as IVFVideoEffectDeinterlaceBlend;
                    }

                    if (blend == null)
                    {
                        MessageBox.Show("Unable to configure deinterlace blend effect.");
                        return;
                    }

                    blend.Threshold1 = Convert.ToInt32(edDeintBlendThreshold1.Text);
                    blend.Threshold2 = Convert.ToInt32(edDeintBlendThreshold2.Text);
                    blend.Constants1 = Convert.ToInt32(edDeintBlendConstants1.Text) / 10.0;
                    blend.Constants2 = Convert.ToInt32(edDeintBlendConstants2.Text) / 10.0;
                }
                else if (rbDeintCAVTEnabled.Checked)
                {
                    IVFVideoEffectDeinterlaceCAVT cavt;
                    var effect = MediaPlayer1.Video_Effects_Get("DeinterlaceCAVT");
                    if (effect == null)
                    {
                        cavt = new VFVideoEffectDeinterlaceCAVT(rbDeintCAVTEnabled.Checked, Convert.ToInt32(edDeintCAVTThreshold.Text));
                        MediaPlayer1.Video_Effects_Add(cavt);
                    }
                    else
                    {
                        cavt = effect as IVFVideoEffectDeinterlaceCAVT;
                    }

                    if (cavt == null)
                    {
                        MessageBox.Show("Unable to configure deinterlace CAVT effect.");
                        return;
                    }

                    cavt.Threshold = Convert.ToInt32(edDeintCAVTThreshold.Text);
                }
                else
                {
                    IVFVideoEffectDeinterlaceTriangle triangle;
                    var effect = MediaPlayer1.Video_Effects_Get("DeinterlaceTriangle");
                    if (effect == null)
                    {
                        triangle = new VFVideoEffectDeinterlaceTriangle(true, Convert.ToByte(edDeintTriangleWeight.Text));
                        MediaPlayer1.Video_Effects_Add(triangle);
                    }
                    else
                    {
                        triangle = effect as IVFVideoEffectDeinterlaceTriangle;
                    }

                    if (triangle == null)
                    {
                        MessageBox.Show("Unable to configure deinterlace triangle effect.");
                        return;
                    }

                    triangle.Weight = Convert.ToByte(edDeintTriangleWeight.Text);
                }
            }

            // Denoise
            if (cbDenoise.Checked)
            {
                MediaPlayer1.Video_Effects_Enabled = true;
                if (rbDenoiseCAST.Checked)
                {
                    IVFVideoEffectDenoiseCAST cast;
                    var effect = MediaPlayer1.Video_Effects_Get("DenoiseCAST");
                    if (effect == null)
                    {
                        cast = new VFVideoEffectDenoiseCAST(true);
                        MediaPlayer1.Video_Effects_Add(cast);
                    }
                    else
                    {
                        cast = effect as IVFVideoEffectDenoiseCAST;
                    }

                    if (cast == null)
                    {
                        MessageBox.Show("Unable to configure denoise CAST effect.");
                        return;
                    }
                }
                else
                {
                    IVFVideoEffectDenoiseMosquito mosquito;
                    var effect = MediaPlayer1.Video_Effects_Get("DenoiseMosquito");
                    if (effect == null)
                    {
                        mosquito = new VFVideoEffectDenoiseMosquito(true);
                        MediaPlayer1.Video_Effects_Add(mosquito);
                    }
                    else
                    {
                        mosquito = effect as IVFVideoEffectDenoiseMosquito;
                    }

                    if (mosquito == null)
                    {
                        MessageBox.Show("Unable to configure denoise mosquito effect.");
                        return;
                    }
                }
            }

            // Other effects
            if (tbLightness.Value > 0)
            {
                tbLightness_Scroll(null, null);
            }

            if (tbSaturation.Value < 255)
            {
                tbSaturation_Scroll(null, null);
            }

            if (tbContrast.Value > 0)
            {
                tbContrast_Scroll(null, null);
            }

            if (tbDarkness.Value > 0)
            {
                tbDarkness_Scroll(null, null);
            }

            if (cbGreyscale.Checked)
            {
                cbGreyscale_CheckedChanged(null, null);
            }

            if (cbInvert.Checked)
            {
                cbInvert_CheckedChanged(null, null);
            }

            if (cbZoom.Checked)
            {
                cbZoom_CheckedChanged(null, null);
            }

            if (cbLiveRotation.Checked)
            {
                cbLiveRotation_CheckedChanged(null, null);
            }

            if (cbPan.Checked)
            {
                cbPan_CheckedChanged(null, null);
            }

            if (cbImageLogo.Checked)
            {
                cbImageLogo_CheckedChanged(null, null);
            }

            if (cbTextLogo.Checked)
            {
                btTextLogoUpdateParams_Click(null, null);
            }

            if (cbFadeInOut.Checked)
            {
                cbFadeInOut_CheckedChanged(null, null);
            }

            // Motion detection
            if (cbMotDetEnabled.Checked)
            {
                btMotDetUpdateSettings_Click(null, null);
            }

            // Barcode detection
            MediaPlayer1.Barcode_Reader_Enabled = cbBarcodeDetectionEnabled.Checked;
            MediaPlayer1.Barcode_Reader_Type = (VFBarcodeType)cbBarcodeType.SelectedIndex;

            // Encryption
            if (rbEncryptionKeyString.Checked)
            {
                MediaPlayer1.Encryption_KeyType = VFEncryptionKeyType.String;
                MediaPlayer1.Encryption_Key = edEncryptionKeyString.Text;
            }
            else if (rbEncryptionKeyFile.Checked)
            {
                MediaPlayer1.Encryption_KeyType = VFEncryptionKeyType.File;
                MediaPlayer1.Encryption_Key = edEncryptionKeyFile.Text;
            }
            else
            {
                MediaPlayer1.Encryption_KeyType = VFEncryptionKeyType.Binary;
                MediaPlayer1.Encryption_Key = VideoCapture.ConvertHexStringToByteArray(edEncryptionKeyHEX.Text);
            }

            // Motion detection-ex
            ConfigureMotionDetectionEx();

            MediaPlayer1.Video_Sample_Grabber_UseForVideoEffects = true;

            MediaPlayer1.Play();

            FillAdjustRanges();

            // DVD
            if (MediaPlayer1.Source_Mode == VFMediaPlayerSource.DVD_DS)
            {
                // select and play first title
                if (cbDVDControlTitle.Items.Count > 0)
                {
                    cbDVDControlTitle.SelectedIndex = 0;
                    cbDVDControlTitle_SelectedIndexChanged(null, null);
                }

                // show title menu
                MediaPlayer1.DVD_Menu_Show(VFDVDMenu.Title);
            }

            // set audio volume for each stream
            if (MediaPlayer1.Source_Mode != VFMediaPlayerSource.DVD_DS && MediaPlayer1.Source_Mode != VFMediaPlayerSource.MMS_WMV_DS)
            {
                int count = MediaPlayer1.Audio_Streams_Count();

                if (count > 0)
                {
                    MediaPlayer1.Audio_OutputDevice_Balance_Set(0, tbBalance1.Value);
                    MediaPlayer1.Audio_OutputDevice_Volume_Set(0, tbVolume1.Value);
                }

                // independent audio output for each audio stream
                if (!MediaPlayer1.Audio_Streams_AllInOne())
                {
                    if (count > 1)
                    {
                        MediaPlayer1.Audio_OutputDevice_Balance_Set(1, tbBalance2.Value);
                        MediaPlayer1.Audio_OutputDevice_Volume_Set(1, tbVolume2.Value);
                        MediaPlayer1.Audio_Streams_Set(1, false); // disable stream
                    }

                    if (count > 2)
                    {
                        MediaPlayer1.Audio_OutputDevice_Balance_Set(2, tbBalance3.Value);
                        MediaPlayer1.Audio_OutputDevice_Volume_Set(2, tbVolume3.Value);
                        MediaPlayer1.Audio_Streams_Set(2, false); // disable stream
                    }

                    if (count > 3)
                    {
                        MediaPlayer1.Audio_OutputDevice_Balance_Set(3, tbBalance4.Value);
                        MediaPlayer1.Audio_OutputDevice_Volume_Set(3, tbVolume4.Value);
                        MediaPlayer1.Audio_Streams_Set(3, false); // disable stream
                    }
                }
                else
                {
                    tbBalance2.Enabled = false;
                    tbVolume2.Enabled = false;

                    tbBalance3.Enabled = false;
                    tbVolume3.Enabled = false;

                    tbBalance4.Enabled = false;
                    tbVolume4.Enabled = false;
                }
            }
            else
            {
                MediaPlayer1.Audio_OutputDevice_Balance_Set(0, tbBalance1.Value);
                MediaPlayer1.Audio_OutputDevice_Volume_Set(0, tbVolume1.Value);
            }

            timer1.Enabled = true;
        }

        private void btSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                edFilenameOrURL.Text = openFileDialog1.FileName;
            }
        }

        private void btDVDControlRootMenu_Click(object sender, EventArgs e)
        {
            MediaPlayer1.DVD_Menu_Show(VFDVDMenu.Root);
        }

        private void btDVDControlTitleMenu_Click(object sender, EventArgs e)
        {
            MediaPlayer1.DVD_Menu_Show(VFDVDMenu.Title);
        }

        private void btZoomIn_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer.Zoom_Ratio = MediaPlayer1.Video_Renderer.Zoom_Ratio + 10;
            MediaPlayer1.Video_Renderer_Update();
        }

        private void btZoomOut_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer.Zoom_Ratio = MediaPlayer1.Video_Renderer.Zoom_Ratio - 10;
            MediaPlayer1.Video_Renderer_Update();
        }

        private void btZoomShiftDown_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer.Zoom_ShiftY = MediaPlayer1.Video_Renderer.Zoom_ShiftY - 10;
            MediaPlayer1.Video_Renderer_Update();
        }

        private void btZoomShiftLeft_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer.Zoom_ShiftX = MediaPlayer1.Video_Renderer.Zoom_ShiftX - 10;
            MediaPlayer1.Video_Renderer_Update();
        }

        private void btZoomShiftRight_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer.Zoom_ShiftX = MediaPlayer1.Video_Renderer.Zoom_ShiftX + 10;
            MediaPlayer1.Video_Renderer_Update();
        }

        private void btZoomShiftUp_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer.Zoom_ShiftY = MediaPlayer1.Video_Renderer.Zoom_ShiftY + 10;
            MediaPlayer1.Video_Renderer_Update();
        }

        private void cbAudioStream1_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Streams_Set(0, cbAudioStream1.Checked);
            if (cbAudioStream1.Checked)
            {
                tbVolume1_Scroll(null, null);

                if (MediaPlayer1.Audio_Streams_AllInOne())
                {
                    cbAudioStream2.Checked = false;
                    cbAudioStream3.Checked = false;
                    cbAudioStream4.Checked = false;
                }
            }
        }

        private void cbAudioStream2_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Streams_Set(1, cbAudioStream2.Checked);
            if (cbAudioStream2.Checked)
            {
                tbVolume2_Scroll(null, null);

                if (MediaPlayer1.Audio_Streams_AllInOne())
                {
                    cbAudioStream1.Checked = false;
                    cbAudioStream3.Checked = false;
                    cbAudioStream4.Checked = false;
                }
            }
        }

        private void cbAudioStream3_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Streams_Set(2, cbAudioStream3.Checked);
            if (cbAudioStream3.Checked)
            {
                tbVolume3_Scroll(null, null);

                if (MediaPlayer1.Audio_Streams_AllInOne())
                {
                    cbAudioStream1.Checked = false;
                    cbAudioStream2.Checked = false;
                    cbAudioStream4.Checked = false;
                }
            }
        }

        private void cbAudioStream4_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Streams_Set(3, cbAudioStream4.Checked);
            if (cbAudioStream4.Checked)
            {
                tbVolume4_Scroll(null, null);

                if (MediaPlayer1.Audio_Streams_AllInOne())
                {
                    cbAudioStream1.Checked = false;
                    cbAudioStream2.Checked = false;
                    cbAudioStream3.Checked = false;
                }
            }
        }

        private void cbDVDControlAudio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDVDControlAudio.SelectedIndex > 0)
            {
                MediaPlayer1.DVD_Select_AudioStream(cbDVDControlAudio.SelectedIndex);
            }
        }

        private void cbDVDControlChapter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDVDControlChapter.SelectedIndex > 0)
            {
                MediaPlayer1.DVD_Chapter_Select(cbDVDControlChapter.SelectedIndex);
            }
        }

        private void cbDVDControlSubtitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDVDControlSubtitles.SelectedIndex > 0)
            {
                MediaPlayer1.DVD_Select_SubpictureStream(cbDVDControlSubtitles.SelectedIndex - 1);
            }

            // 0 - x - subtitles
            // -1 - disable subtitles
        }

        private void cbDVDControlTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDVDControlTitle.SelectedIndex != -1)
            {
                // fill info
                cbDVDControlAudio.Items.Clear();
                cbDVDControlSubtitles.Items.Clear();
                cbDVDControlChapter.Items.Clear();

                MediaInfo.DVD_Fill_Title_Info(cbDVDControlTitle.SelectedIndex);

                for (int i = 0; i < MediaInfo.DVD_Title_NumberOfChapters; i++)
                {
                    cbDVDControlChapter.Items.Add("Chapter " + Convert.ToString(i + 1));
                }

                if (cbDVDControlChapter.Items.Count > 0)
                {
                    cbDVDControlChapter.SelectedIndex = 0;
                }

                for (int i = 0; i < MediaInfo.DVD_Title_MainAttributes_NumberOfAudioStreams; i++)
                {
                    MediaInfo.DVD_Fill_Title_Audio_Info(cbDVDControlTitle.SelectedIndex, i);
                    string s = MediaInfo.DVD_Title_MainAttributes_AudioAttributes_AudioFormat;

                    s = s + " - ";
                    s = s + MediaInfo.DVD_Title_MainAttributes_AudioAttributes_NumberOfChannels + "ch" + " - ";
                    s = s + MediaInfo.DVD_Title_MainAttributes_AudioAttributes_LanguageS;

                    cbDVDControlAudio.Items.Add(s);
                }

                if (cbDVDControlAudio.Items.Count > 0)
                {
                    cbDVDControlAudio.SelectedIndex = 0;
                }

                cbDVDControlSubtitles.Items.Add("Disabled");
                for (int i = 0; i < MediaInfo.DVD_Title_MainAttributes_NumberOfSubpictureStreams; i++)
                {
                    MediaInfo.DVD_Fill_Title_Subpicture_Info(cbDVDControlTitle.SelectedIndex, i);
                    cbDVDControlSubtitles.Items.Add(MediaInfo.DVD_Title_MainAttributes_SubpictureAttributes_LanguageS);
                }

                cbDVDControlSubtitles.SelectedIndex = 0;

                // if null we just enumerate titles and chapters
                if (sender != null)
                {
                    // play title
                    MediaPlayer1.DVD_Title_Play(cbDVDControlTitle.SelectedIndex);
                    tbTimeline.Maximum = MediaPlayer1.DVD_Title_GetDurationS();
                }
            }
        }

        private void cbScreenFlipHorizontal_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer.Flip_Horizontal = cbScreenFlipHorizontal.Checked;
            MediaPlayer1.Video_Renderer_Update();
        }

        private void cbScreenFlipVertical_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer.Flip_Vertical = cbScreenFlipVertical.Checked;
            MediaPlayer1.Video_Renderer_Update();
        }

        private void cbStretch_CheckedChanged(object sender, EventArgs e)
        {
            if (cbStretch.Checked)
            {
                MediaPlayer1.Video_Renderer.StretchMode = VFVideoRendererStretchMode.Stretch;
            }
            else
            {
                MediaPlayer1.Video_Renderer.StretchMode = VFVideoRendererStretchMode.Letterbox;
            }

            MediaPlayer1.Video_Renderer_Update();
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Stop();
            timer1.Enabled = false;
            tbTimeline.Value = 0;

            waveformPainter1.Clear();
            waveformPainter2.Clear();

            volumeMeter1.Clear();
            volumeMeter2.Clear();

            if (cbMultiscreenDrawOnPanels.Checked)
            {
                pnScreen1.Refresh();
                pnScreen2.Refresh();
            }

            foreach (var form in multiscreenWindows)
            {
                form.Close();
                form.Dispose();
            }

            multiscreenWindows.Clear();
        }

        private void btResume_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Resume();
        }

        private void btPause_Click(object sender, EventArgs e)
        {
            MediaPlayer1.Pause();
        }

        private void btNextFrame_Click(object sender, EventArgs e)
        {
            MediaPlayer1.NextFrame();
        }

        private FileStream memoryFileStream;

        private void LoadToMemory()
        {
            if (memoryFileStream != null)
            {
                memoryFileStream.Dispose();
                memoryFileStream = null;
            }

            memoryFileStream = new FileStream(edFilenameOrURL.Text, FileMode.Open);
            ManagedIStream stream = new ManagedIStream(memoryFileStream);

            // specifing settings
            // MediaPlayer1.Source_Mode = VFMediaPlayerSource.Memory_DS;
            MediaPlayer1.Source_Stream = stream;
            MediaPlayer1.Source_Stream_Size = memoryFileStream.Length;

            // video and audio present in file. tune this settings to play audio files or video files without audio
            MediaPlayer1.Source_Stream_VideoPresent = true;
            MediaPlayer1.Source_Stream_AudioPresent = true;
        }

        private void tbContrast_Scroll(object sender, EventArgs e)
        {
            IVFVideoEffectContrast contrast;
            var effect = MediaPlayer1.Video_Effects_Get("Contrast");
            if (effect == null)
            {
                contrast = new VFVideoEffectContrast(true, tbContrast.Value);
                MediaPlayer1.Video_Effects_Add(contrast);
            }
            else
            {
                contrast = effect as IVFVideoEffectContrast;
                if (contrast != null)
                {
                    contrast.Value = tbContrast.Value;
                }
            }
        }

        private void cbAspectRatioUseCustom_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer.Aspect_Ratio_Override = cbAspectRatioUseCustom.Checked;
            MediaPlayer1.Video_Renderer.Aspect_Ratio_X = Convert.ToInt32(edAspectRatioX.Text);
            MediaPlayer1.Video_Renderer.Aspect_Ratio_Y = Convert.ToInt32(edAspectRatioY.Text);
            MediaPlayer1.Video_Renderer_Update();
        }

        private void btAudEqRefresh_Click(object sender, EventArgs e)
        {
            tbAudEq0.Value = MediaPlayer1.Audio_Effects_Equalizer_Band_Get(-1, 1, 0);
            tbAudEq1.Value = MediaPlayer1.Audio_Effects_Equalizer_Band_Get(-1, 1, 1);
            tbAudEq2.Value = MediaPlayer1.Audio_Effects_Equalizer_Band_Get(-1, 1, 2);
            tbAudEq3.Value = MediaPlayer1.Audio_Effects_Equalizer_Band_Get(-1, 1, 3);
            tbAudEq4.Value = MediaPlayer1.Audio_Effects_Equalizer_Band_Get(-1, 1, 4);
            tbAudEq5.Value = MediaPlayer1.Audio_Effects_Equalizer_Band_Get(-1, 1, 5);
            tbAudEq6.Value = MediaPlayer1.Audio_Effects_Equalizer_Band_Get(-1, 1, 6);
            tbAudEq7.Value = MediaPlayer1.Audio_Effects_Equalizer_Band_Get(-1, 1, 7);
            tbAudEq8.Value = MediaPlayer1.Audio_Effects_Equalizer_Band_Get(-1, 1, 8);
            tbAudEq9.Value = MediaPlayer1.Audio_Effects_Equalizer_Band_Get(-1, 1, 9);
        }

        private void cbAudAmplifyEnabled_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Enable(-1, 0, cbAudAmplifyEnabled.Checked);
            MediaPlayer1.Audio_Effects_Amplify(-1, 0, tbAudAmplifyAmp.Value * 10, false);
        }

        private void cbAudDynamicAmplifyEnabled_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Enable(-1, 2, cbAudDynamicAmplifyEnabled.Checked);
        }

        private void cbAudEqualizerEnabled_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Enable(-1, 1, cbAudEqualizerEnabled.Checked);
        }

        private void cbAudEqualizerPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Equalizer_Preset_Set(-1, 1, (EqualizerPreset)cbAudEqualizerPreset.SelectedIndex);
            btAudEqRefresh_Click(null, null);
        }

        private void cbAudSound3DEnabled_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Enable(-1, 3, cbAudSound3DEnabled.Checked);
        }

        private void cbAudTrueBassEnabled_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Enable(-1, 4, cbAudTrueBassEnabled.Checked);
        }

        private void pnTextLogoBGColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = pnTextLogoGradColor1.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pnTextLogoGradColor1.BackColor = colorDialog1.Color;
            }
        }

        private void pnTextLogoGradColor1_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = pnTextLogoGradColor1.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pnTextLogoGradColor1.BackColor = colorDialog1.Color;
            }
        }

        private void pnTextLogoGradColor2_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = pnTextLogoGradColor2.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pnTextLogoGradColor2.BackColor = colorDialog1.Color;
            }
        }

        private void pnTextLogoInnerColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = pnTextLogoInnerColor.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pnTextLogoInnerColor.BackColor = colorDialog1.Color;
            }
        }

        private void pnTextLogoOuterColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = pnTextLogoOuterColor.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pnTextLogoOuterColor.BackColor = colorDialog1.Color;
            }
        }

        private void pnTextLogoShapeColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = pnTextLogoShapeColor.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pnTextLogoShapeColor.BackColor = colorDialog1.Color;
            }
        }

        private void tbAud3DSound_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Sound3D(-1, 3, tbAud3DSound.Value);
        }

        private void tbAudAmplifyAmp_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Amplify(-1, 0, tbAudAmplifyAmp.Value * 10, false);
        }

        private void tbAudAttack_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_DynamicAmplify(-1, 2, tbAudAttack.Value, tbAudDynAmp.Value, tbAudRelease.Value);
        }

        private void tbAudDynAmp_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_DynamicAmplify(-1, 2, tbAudAttack.Value, tbAudDynAmp.Value, tbAudRelease.Value);
        }

        private void tbAudEq0_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Equalizer_Band_Set(-1, 1, 0, tbAudEq0.Value);
        }

        private void tbAudEq1_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Equalizer_Band_Set(-1, 1, 1, tbAudEq1.Value);
        }

        private void tbAudEq2_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Equalizer_Band_Set(-1, 1, 2, tbAudEq2.Value);
        }

        private void tbAudEq3_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Equalizer_Band_Set(-1, 1, 3, tbAudEq3.Value);
        }

        private void tbAudEq4_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Equalizer_Band_Set(-1, 1, 4, tbAudEq4.Value);
        }

        private void tbAudEq5_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Equalizer_Band_Set(-1, 1, 5, tbAudEq5.Value);
        }

        private void tbAudEq6_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Equalizer_Band_Set(-1, 1, 6, tbAudEq6.Value);
        }

        private void tbAudEq7_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Equalizer_Band_Set(-1, 1, 7, tbAudEq7.Value);
        }

        private void tbAudEq8_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Equalizer_Band_Set(-1, 1, 8, tbAudEq8.Value);
        }

        private void tbAudEq9_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Equalizer_Band_Set(-1, 1, 9, tbAudEq9.Value);
        }

        private void tbAudRelease_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_DynamicAmplify(-1, 2, tbAudAttack.Value, tbAudDynAmp.Value, tbAudRelease.Value);
        }

        private void tbAudTrueBass_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_TrueBass(-1, 4, 200, false, tbAudTrueBass.Value);
        }

        private void tbJPEGQuality_Scroll(object sender, EventArgs e)
        {
            lbJPEGQuality.Text = tbJPEGQuality.Value.ToString(CultureInfo.InvariantCulture);
        }

        private void MediaPlayer1_OnError(object sender, ErrorsEventArgs e)
        {
            mmLog.Text = mmLog.Text + e.Message + Environment.NewLine;
        }

        private void MediaPlayer1_OnStop(object sender, MediaPlayerStopEventArgs e)
        {
            BeginInvoke(new StopDelegate(StopDelegateMethod), null);
        }

        private delegate void StopDelegate();

        private void StopDelegateMethod()
        {
            tbTimeline.Value = 0;

            waveformPainter1.Clear();
            waveformPainter2.Clear();

            volumeMeter1.Clear();
            volumeMeter2.Clear();

            if (memoryFileStream != null)
            {
                memoryFileStream.Dispose();
                memoryFileStream = null;
            }
        }

        private void tbAdjBrightness_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer_VideoAdjust_SetValue(VFVideoRendererAdjustment.Brightness, (float)(tbAdjBrightness.Value / 10.0));
            lbAdjBrightnessCurrent.Text = "Current: " + Convert.ToString(tbAdjBrightness.Value / 10.0, CultureInfo.InvariantCulture);
        }

        private void tbAdjContrast_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer_VideoAdjust_SetValue(VFVideoRendererAdjustment.Contrast, (float)(tbAdjContrast.Value / 10.0));
            lbAdjContrastCurrent.Text = "Current: " + Convert.ToString(tbAdjContrast.Value / 10.0, CultureInfo.InvariantCulture);
        }

        private void tbAdjHue_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer_VideoAdjust_SetValue(VFVideoRendererAdjustment.Hue, (float)(tbAdjHue.Value / 10.0));
            lbAdjHueCurrent.Text = "Current: " + Convert.ToString(tbAdjHue.Value / 10.0, CultureInfo.InvariantCulture);
        }

        private void tbAdjSaturation_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer_VideoAdjust_SetValue(VFVideoRendererAdjustment.Saturation, (float)(tbAdjSaturation.Value / 10.0));
            lbAdjSaturationCurrent.Text = "Current: " + Convert.ToString(tbAdjSaturation.Value / 10.0, CultureInfo.InvariantCulture);
        }

        private void btMotDetUpdateSettings_Click(object sender, EventArgs e)
        {
            if (cbMotDetEnabled.Checked)
            {
                MediaPlayer1.Motion_Detection = new MotionDetectionSettings
                {
                    Enabled = cbMotDetEnabled.Checked,
                    Compare_Red = cbCompareRed.Checked,
                    Compare_Green = cbCompareGreen.Checked,
                    Compare_Blue = cbCompareBlue.Checked,
                    Compare_Greyscale = cbCompareGreyscale.Checked,
                    Highlight_Color = (VFMotionCHLColor)cbMotDetHLColor.SelectedIndex,
                    Highlight_Enabled = cbMotDetHLEnabled.Checked,
                    Highlight_Threshold = tbMotDetHLThreshold.Value,
                    FrameInterval = Convert.ToInt32(edMotDetFrameInterval.Text),
                    Matrix_Height = Convert.ToInt32(edMotDetMatrixHeight.Text),
                    Matrix_Width = Convert.ToInt32(edMotDetMatrixWidth.Text),
                    DropFrames_Enabled = cbMotDetDropFramesEnabled.Checked,
                    DropFrames_Threshold = tbMotDetDropFramesThreshold.Value
                };
                MediaPlayer1.MotionDetection_Update();
            }
            else
            {
                MediaPlayer1.Motion_Detection = null;
            }
        }

        private void btChromaKeySelectBGImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                edChromaKeyImage.Text = openFileDialog1.FileName;
            }
        }

        private void ConfigureMotionDetectionEx()
        {
            if (cbMotionDetectionEx.Checked)
            {
                MediaPlayer1.Motion_DetectionEx = new MotionDetectionExSettings();
                MediaPlayer1.Motion_DetectionEx.ProcessorType = (MotionProcessorType)rbMotionDetectionExProcessor.SelectedIndex;
                MediaPlayer1.Motion_DetectionEx.DetectorType = (MotionDetectorType)rbMotionDetectionExDetector.SelectedIndex;
            }
            else
            {
                MediaPlayer1.Motion_DetectionEx = null;
            }
        }

        private delegate void AFMotionDelegate(float level);

        private void AFMotionDelegateMethod(float level)
        {
            pbAFMotionLevel.Value = (int)(level * 100);
        }

        private void MediaPlayer1_OnObjectDetection(object sender, MotionDetectionExEventArgs e)
        {
            BeginInvoke(new AFMotionDelegate(AFMotionDelegateMethod), e.Level);
        }

        private void tbChromaKeyContrastLow_Scroll(object sender, EventArgs e)
        {
            ConfigureChromaKey();
        }

        private void tbChromaKeyContrastHigh_Scroll(object sender, EventArgs e)
        {
            ConfigureChromaKey();
        }

        public delegate void MotionDelegate(MotionDetectionEventArgs e);

        public void MotionDelegateMethod(MotionDetectionEventArgs e)
        {
            string s = string.Empty;
            int k = 0;
            foreach (byte b in e.Matrix)
            {
                s += b.ToString("D3") + " ";

                if (k == MediaPlayer1.Motion_Detection.Matrix_Width - 1)
                {
                    k = 0;
                    s += Environment.NewLine;
                }
                else
                {
                    k++;
                }
            }

            mmMotDetMatrix.Text = s.Trim();
            pbMotionLevel.Value = e.Level;
        }

        private void MediaPlayer1_OnMotion(object sender, MotionDetectionEventArgs e)
        {
            BeginInvoke(new MotionDelegate(MotionDelegateMethod), e);
        }

        private void rbVR_CheckedChanged(object sender, EventArgs e)
        {
            cbScreenFlipVertical.Enabled = rbVMR9.Checked || rbDirect2D.Checked;
            cbScreenFlipHorizontal.Enabled = rbVMR9.Checked || rbDirect2D.Checked;

            if (rbVMR9.Checked)
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.VMR9;
            }
            else if (rbEVR.Checked)
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.EVR;
            }
            else if (rbVR.Checked)
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.VideoRenderer;
            }
            else if (rbDirect2D.Checked)
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.Direct2D;
            }
            else
            {
                MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.None;
            }
        }

        private void cbStretch1_CheckedChanged(object sender, EventArgs e)
        {
            VFVideoRendererStretchMode stretch;
            if (cbStretch1.Checked)
            {
                stretch = VFVideoRendererStretchMode.Stretch;
            }
            else
            {
                stretch = VFVideoRendererStretchMode.Letterbox;
            }

            MediaPlayer1.MultiScreen_SetParameters(0, stretch, cbFlipHorizontal1.Checked, cbFlipVertical1.Checked);
        }

        private void cbStretch2_CheckedChanged(object sender, EventArgs e)
        {
            VFVideoRendererStretchMode stretch;
            if (cbStretch2.Checked)
            {
                stretch = VFVideoRendererStretchMode.Stretch;
            }
            else
            {
                stretch = VFVideoRendererStretchMode.Letterbox;
            }

            MediaPlayer1.MultiScreen_SetParameters(1, stretch, cbFlipHorizontal2.Checked, cbFlipVertical2.Checked);
        }

        private void cbFlipVertical1_CheckedChanged(object sender, EventArgs e)
        {
            VFVideoRendererStretchMode stretch;
            if (cbStretch1.Checked)
            {
                stretch = VFVideoRendererStretchMode.Stretch;
            }
            else
            {
                stretch = VFVideoRendererStretchMode.Letterbox;
            }

            MediaPlayer1.MultiScreen_SetParameters(0, stretch, cbFlipHorizontal1.Checked, cbFlipVertical1.Checked);
        }

        private void cbFlipHorizontal2_CheckedChanged(object sender, EventArgs e)
        {
            VFVideoRendererStretchMode stretch;
            if (cbStretch2.Checked)
            {
                stretch = VFVideoRendererStretchMode.Stretch;
            }
            else
            {
                stretch = VFVideoRendererStretchMode.Letterbox;
            }

            MediaPlayer1.MultiScreen_SetParameters(1, stretch, cbFlipHorizontal2.Checked, cbFlipVertical2.Checked);
        }

        private void cbFlipHorizontal1_CheckedChanged(object sender, EventArgs e)
        {
            VFVideoRendererStretchMode stretch;
            if (cbStretch1.Checked)
            {
                stretch = VFVideoRendererStretchMode.Stretch;
            }
            else
            {
                stretch = VFVideoRendererStretchMode.Letterbox;
            }

            MediaPlayer1.MultiScreen_SetParameters(0, stretch, cbFlipHorizontal1.Checked, cbFlipVertical1.Checked);
        }

        private void cbFlipVertical2_CheckedChanged(object sender, EventArgs e)
        {
            VFVideoRendererStretchMode stretch;
            if (cbStretch2.Checked)
            {
                stretch = VFVideoRendererStretchMode.Stretch;
            }
            else
            {
                stretch = VFVideoRendererStretchMode.Letterbox;
            }

            MediaPlayer1.MultiScreen_SetParameters(1, stretch, cbFlipHorizontal2.Checked, cbFlipVertical2.Checked);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (MediaPlayer1.Status != VFMediaPlayerStatus.Free)
            {
                MediaPlayer1.Stop();
            }
        }

        private void btTest_Click(object sender, EventArgs e)
        {
            var indexer = new ASFIndexer();
            indexer.OnStop += delegate(object o, EventArgs args)
            {
                MessageBox.Show("Indexing complete.");
            };

            indexer.OnError += delegate(object o, ErrorsEventArgs args)
                {
                    MessageBox.Show("Error during indexing: " + args.Message);
                };

            indexer.OnProgress += delegate(object o, ProgressEventArgs args)
            {
                Debug.WriteLine("Indexing progress: " + args.Progress);
            };

            indexer.Start(@"c:\samples\!_LH CAB FRONT 2018-09-10_12_00_03_862.asf", VFWMIndexerType.FrameNumbers, 4000, VFWMIndexType.NearestDataUnit);
        }

        private void cbZoom_CheckedChanged(object sender, EventArgs e)
        {
            IVFVideoEffectZoom zoomEffect;
            var effect = MediaPlayer1.Video_Effects_Get("Zoom");
            if (effect == null)
            {
                zoomEffect = new VFVideoEffectZoom(zoom, zoom, zoomShiftX, zoomShiftY, cbZoom.Checked);
                MediaPlayer1.Video_Effects_Add(zoomEffect);
            }
            else
            {
                zoomEffect = effect as IVFVideoEffectZoom;
            }

            if (zoomEffect == null)
            {
                MessageBox.Show("Unable to configure zoom effect.");
                return;
            }

            zoomEffect.ZoomX = zoom;
            zoomEffect.ZoomY = zoom;
            zoomEffect.ShiftX = zoomShiftX;
            zoomEffect.ShiftY = zoomShiftY;
            zoomEffect.Enabled = cbZoom.Checked;
        }

        private void btEffZoomIn_Click(object sender, EventArgs e)
        {
            zoom += 0.1;
            zoom = Math.Min(zoom, 5);

            cbZoom_CheckedChanged(null, null);
        }

        private void btEffZoomOut_Click(object sender, EventArgs e)
        {
            zoom -= 0.1;
            zoom = Math.Max(zoom, 1);

            cbZoom_CheckedChanged(null, null);
        }

        private void btEffZoomUp_Click(object sender, EventArgs e)
        {
            zoomShiftY += 20;

            cbZoom_CheckedChanged(null, null);
        }

        private void btEffZoomDown_Click(object sender, EventArgs e)
        {
            zoomShiftY -= 20;

            cbZoom_CheckedChanged(null, null);
        }

        private void btEffZoomRight_Click(object sender, EventArgs e)
        {
            zoomShiftX += 20;

            cbZoom_CheckedChanged(null, null);
        }

        private void btEffZoomLeft_Click(object sender, EventArgs e)
        {
            zoomShiftX -= 20;

            cbZoom_CheckedChanged(null, null);
        }

        private void cbPan_CheckedChanged(object sender, EventArgs e)
        {
            IVFVideoEffectPan pan;
            var effect = MediaPlayer1.Video_Effects_Get("Pan");
            if (effect == null)
            {
                pan = new VFVideoEffectPan(true);
                MediaPlayer1.Video_Effects_Add(pan);
            }
            else
            {
                pan = effect as IVFVideoEffectPan;
            }

            if (pan == null)
            {
                MessageBox.Show("Unable to configure pan effect.");
                return;
            }

            pan.Enabled = cbPan.Checked;
            pan.StartTime = Convert.ToInt32(edPanStartTime.Text);
            pan.StopTime = Convert.ToInt32(edPanStopTime.Text);
            pan.StartX = Convert.ToInt32(edPanSourceLeft.Text);
            pan.StartY = Convert.ToInt32(edPanSourceTop.Text);
            pan.StartWidth = Convert.ToInt32(edPanSourceWidth.Text);
            pan.StartHeight = Convert.ToInt32(edPanSourceHeight.Text);
            pan.StopX = Convert.ToInt32(edPanDestLeft.Text);
            pan.StopY = Convert.ToInt32(edPanDestTop.Text);
            pan.StopWidth = Convert.ToInt32(edPanDestWidth.Text);
            pan.StopHeight = Convert.ToInt32(edPanDestHeight.Text);
        }

        private void btBarcodeReset_Click(object sender, EventArgs e)
        {
            edBarcode.Text = string.Empty;
            edBarcodeMetadata.Text = string.Empty;
            MediaPlayer1.Barcode_Reader_Enabled = true;
        }

        #region Barcode detector

        public delegate void BarcodeDelegate(BarcodeEventArgs value);

        public void BarcodeDelegateMethod(BarcodeEventArgs value)
        {
            edBarcode.Text = value.Value;
            edBarcodeMetadata.Text = string.Empty;
            foreach (var o in value.Metadata)
            {
                edBarcodeMetadata.Text += o.Key + ": " + o.Value + Environment.NewLine;
            }
        }

        private void MediaPlayer1_OnBarcodeDetected(object sender, BarcodeEventArgs e)
        {
            e.DetectorEnabled = false;

            BeginInvoke(new BarcodeDelegate(BarcodeDelegateMethod), e);
        }

        #endregion

        private void btAddFileToPlaylist_Click(object sender, EventArgs e)
        {
            lbSourceFiles.Items.Add(edFilenameOrURL.Text);
        }

        private void cbFadeInOut_CheckedChanged(object sender, EventArgs e)
        {
            if (rbFadeIn.Checked)
            {
                IVFVideoEffectFadeIn fadeIn;
                var effect = MediaPlayer1.Video_Effects_Get("FadeIn");
                if (effect == null)
                {
                    fadeIn = new VFVideoEffectFadeIn(cbFadeInOut.Checked);
                    MediaPlayer1.Video_Effects_Add(fadeIn);
                }
                else
                {
                    fadeIn = effect as IVFVideoEffectFadeIn;
                }

                if (fadeIn == null)
                {
                    MessageBox.Show("Unable to configure fade-in effect.");
                    return;
                }

                fadeIn.Enabled = cbFadeInOut.Checked;
                fadeIn.StartTime = Convert.ToInt64(edFadeInOutStartTime.Text);
                fadeIn.StopTime = Convert.ToInt64(edFadeInOutStopTime.Text);
            }
            else
            {
                IVFVideoEffectFadeOut fadeOut;
                var effect = MediaPlayer1.Video_Effects_Get("FadeOut");
                if (effect == null)
                {
                    fadeOut = new VFVideoEffectFadeOut(cbFadeInOut.Checked);
                    MediaPlayer1.Video_Effects_Add(fadeOut);
                }
                else
                {
                    fadeOut = effect as IVFVideoEffectFadeOut;
                }

                if (fadeOut == null)
                {
                    MessageBox.Show("Unable to configure fade-out effect.");
                    return;
                }

                fadeOut.Enabled = cbFadeInOut.Checked;
                fadeOut.StartTime = Convert.ToInt64(edFadeInOutStartTime.Text);
                fadeOut.StopTime = Convert.ToInt64(edFadeInOutStopTime.Text);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var startInfo = new ProcessStartInfo("explorer.exe", @"http://www.visioforge.com/video_tutorials");
            Process.Start(startInfo);
        }

        private void MediaPlayer1_OnVideoEncrypted(object sender, EventArgs e)
        {
            MessageBox.Show("Video is encrypted. Please be sure that you're entered correct pin code.");
        }

        #region Full screen

        private bool fullScreen;

        private int windowLeft;

        private int windowTop;

        private int windowWidth;

        private int windowHeight;

        private int controlLeft;

        private int controlTop;

        private int controlWidth;

        private int controlHeight;

        private void btFullScreen_Click(object sender, EventArgs e)
        {
            if (!fullScreen)
            {
                // going fullscreen
                fullScreen = true;

                // saving coordinates
                windowLeft = Left;
                windowTop = Top;
                windowWidth = Width;
                windowHeight = Height;

                controlLeft = MediaPlayer1.Left;
                controlTop = MediaPlayer1.Top;
                controlWidth = MediaPlayer1.Width;
                controlHeight = MediaPlayer1.Height;

                // resizing window
                Left = 0;
                Top = 0;
                Width = Screen.AllScreens[0].WorkingArea.Width;
                Height = Screen.AllScreens[0].WorkingArea.Height;

                TopMost = true;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;

                // resizing control
                MediaPlayer1.Left = 0;
                MediaPlayer1.Top = 0;
                MediaPlayer1.Width = Width;
                MediaPlayer1.Height = Height;

                MediaPlayer1.Video_Renderer_Update();
            }
            else
            {
                // going normal screen
                fullScreen = false;

                // restoring control
                MediaPlayer1.Left = controlLeft;
                MediaPlayer1.Top = controlTop;
                MediaPlayer1.Width = controlWidth;
                MediaPlayer1.Height = controlHeight;

                // restoring window
                Left = windowLeft;
                Top = windowTop;
                Width = windowWidth;
                Height = windowHeight;

                TopMost = false;
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;

                MediaPlayer1.Video_Renderer_Update();
            }
        }

        private void MediaPlayer1_MouseClick(object sender, MouseEventArgs e)
        {
            if (fullScreen)
            {
                btFullScreen_Click(null, null);
            }
        }

        #endregion

        private void tbReversePlaybackTrackbar_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.ReversePlayback_GoToFrame(tbReversePlaybackTrackbar.Value);
        }

        private void btReversePlayback_Click(object sender, EventArgs e)
        {
            MediaPlayer1.ReversePlayback_CacheSize = int.Parse(edReversePlaybackCacheSize.Text);

            if (MediaPlayer1.ReversePlayback_Enabled)
            {
                btReversePlayback.Text = "Go to reverse playback mode";
                MediaPlayer1.ReversePlayback_Enabled = false;
            }
            else
            {
                btReversePlayback.Text = "Go to normal playback mode";
                MediaPlayer1.ReversePlayback_Enabled = true;
            }
        }

        private void tbAudPitchShift_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_PitchShift(-1, 5, tbAudPitchShift.Value / 1000f);
        }

        private void cbAudPitchShiftEnabled_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Effects_Enable(-1, 5, cbAudPitchShiftEnabled.Checked);
        }

        private void MediaPlayer1_OnAudioVUMeterProVolume(object sender, AudioLevelEventArgs e)
        {
            volumeMeter1.Amplitude = e.ChannelLevelsDb[0];
            waveformPainter1.AddMax(e.ChannelLevelsDb[0]);

            if (e.ChannelLevelsDb.Length > 1)
            {
                volumeMeter2.Amplitude = e.ChannelLevelsDb[1];
                waveformPainter2.AddMax(e.ChannelLevelsDb[1]);
            }
        }

        private void tbVUMeterAmplification_Scroll(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_VUMeter_Pro_Volume = tbVUMeterAmplification.Value;
        }

        private void tbVUMeterBoost_Scroll(object sender, EventArgs e)
        {
            volumeMeter1.Boost = tbVUMeterBoost.Value / 10.0F;
            volumeMeter2.Boost = tbVUMeterBoost.Value / 10.0F;

            waveformPainter1.Boost = tbVUMeterBoost.Value / 10.0F;
            waveformPainter2.Boost = tbVUMeterBoost.Value / 10.0F;
        }

        private void cbLiveRotation_CheckedChanged(object sender, EventArgs e)
        {
            IVFVideoEffectRotate rotate;
            var effect = MediaPlayer1.Video_Effects_Get("Rotate");
            if (effect == null)
            {
                rotate = new VFVideoEffectRotate(
                    cbLiveRotation.Checked,
                    tbLiveRotationAngle.Value,
                    cbLiveRotationStretch.Checked);
                MediaPlayer1.Video_Effects_Add(rotate);
            }
            else
            {
                rotate = effect as IVFVideoEffectRotate;
            }

            if (rotate == null)
            {
                MessageBox.Show("Unable to configure rotate effect.");
                return;
            }

            rotate.Enabled = cbLiveRotation.Checked;
            rotate.Angle = tbLiveRotationAngle.Value;
            rotate.Stretch = cbLiveRotationStretch.Checked;
        }

        private void tbLiveRotationAngle_Scroll(object sender, EventArgs e)
        {
            cbLiveRotation_CheckedChanged(sender, e);
        }

        private void cbLiveRotationStretch_CheckedChanged(object sender, EventArgs e)
        {
            cbLiveRotation_CheckedChanged(sender, e);
        }

        private void pnVideoRendererBGColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = pnVideoRendererBGColor.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pnVideoRendererBGColor.BackColor = colorDialog1.Color;
            }
        }

        private void cbDirect2DRotate_SelectedIndexChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Video_Renderer.RotationAngle = Convert.ToInt32(cbDirect2DRotate.Text);
            MediaPlayer1.Video_Renderer_Update();
        }

        private void cbFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFilters.SelectedIndex != -1)
            {
                string name = cbFilters.Text;
                btFilterSettings.Enabled = MediaPlayer.DirectShow_Filter_Has_Dialog(name, VFPropertyPage.Default) ||
                    MediaPlayer.DirectShow_Filter_Has_Dialog(name, VFPropertyPage.VFWCompConfig);
            }
        }

        private void btFilterAdd_Click(object sender, EventArgs e)
        {
            if (cbFilters.SelectedIndex != -1)
            {
                MediaPlayer1.Video_Filters_Add(cbFilters.Text);
                lbFilters.Items.Add(cbFilters.Text);
            }
        }

        private void btFilterSettings_Click(object sender, EventArgs e)
        {
            string name = cbFilters.Text;

            if (MediaPlayer.DirectShow_Filter_Has_Dialog(name, VFPropertyPage.Default))
            {
                MediaPlayer.DirectShow_Filter_Show_Dialog(IntPtr.Zero, name, VFPropertyPage.Default);
            }
            else if (MediaPlayer.DirectShow_Filter_Has_Dialog(name, VFPropertyPage.VFWCompConfig))
            {
                MediaPlayer.DirectShow_Filter_Show_Dialog(IntPtr.Zero, name, VFPropertyPage.VFWCompConfig);
            }
        }

        private void lbFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbFilters.SelectedIndex != -1)
            {
                string name = lbFilters.Text;
                btFilterSettings2.Enabled = MediaPlayer.DirectShow_Filter_Has_Dialog(name, VFPropertyPage.Default) ||
                                            MediaPlayer.DirectShow_Filter_Has_Dialog(name, VFPropertyPage.VFWCompConfig);
            }
        }

        private void btFilterSettings2_Click(object sender, EventArgs e)
        {
            if (lbFilters.SelectedIndex != -1)
            {
                string name = lbFilters.Text;

                if (MediaPlayer.DirectShow_Filter_Has_Dialog(name, VFPropertyPage.Default))
                {
                    MediaPlayer.DirectShow_Filter_Show_Dialog(IntPtr.Zero, name, VFPropertyPage.Default);
                }
                else if (MediaPlayer.DirectShow_Filter_Has_Dialog(name, VFPropertyPage.VFWCompConfig))
                {
                    MediaPlayer.DirectShow_Filter_Show_Dialog(IntPtr.Zero, name, VFPropertyPage.VFWCompConfig);
                }
            }
        }

        private void btFilterDelete_Click(object sender, EventArgs e)
        {
            if (lbFilters.SelectedIndex != -1)
            {
                MediaPlayer1.Video_Filters_Delete(lbFilters.Text);
                lbFilters.Items.Remove(lbFilters.Text);
            }
        }

        private void btFilterDeleteAll_Click(object sender, EventArgs e)
        {
            lbFilters.Items.Clear();
            MediaPlayer1.Video_Filters_Clear();
        }

        private void cbAudioNormalize_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Enhancer_Normalize(-1, cbAudioNormalize.Checked);
        }

        private void cbAudioAutoGain_CheckedChanged(object sender, EventArgs e)
        {
            MediaPlayer1.Audio_Enhancer_AutoGain(-1, cbAudioAutoGain.Checked);
        }

        private void ApplyAudioInputGains()
        {
            VFAudioEnhancerGains gains = new VFAudioEnhancerGains
            {
                L = tbAudioInputGainL.Value / 10.0f,
                C = tbAudioInputGainC.Value / 10.0f,
                R = tbAudioInputGainR.Value / 10.0f,
                SL = tbAudioInputGainSL.Value / 10.0f,
                SR = tbAudioInputGainSR.Value / 10.0f,
                LFE = tbAudioInputGainLFE.Value / 10.0f
            };

            MediaPlayer1.Audio_Enhancer_InputGains(-1, gains);
        }

        private void tbAudioInputGainL_Scroll(object sender, EventArgs e)
        {
            lbAudioInputGainL.Text = (tbAudioInputGainL.Value / 10.0f).ToString("F1");

            ApplyAudioInputGains();
        }

        private void tbAudioInputGainC_Scroll(object sender, EventArgs e)
        {
            lbAudioInputGainC.Text = (tbAudioInputGainC.Value / 10.0f).ToString("F1");

            ApplyAudioInputGains();
        }

        private void tbAudioInputGainR_Scroll(object sender, EventArgs e)
        {
            lbAudioInputGainR.Text = (tbAudioInputGainR.Value / 10.0f).ToString("F1");

            ApplyAudioInputGains();
        }

        private void tbAudioInputGainSL_Scroll(object sender, EventArgs e)
        {
            lbAudioInputGainSL.Text = (tbAudioInputGainSL.Value / 10.0f).ToString("F1");

            ApplyAudioInputGains();
        }

        private void tbAudioInputGainSR_Scroll(object sender, EventArgs e)
        {
            lbAudioInputGainSR.Text = (tbAudioInputGainSR.Value / 10.0f).ToString("F1");

            ApplyAudioInputGains();
        }

        private void tbAudioInputGainLFE_Scroll(object sender, EventArgs e)
        {
            lbAudioInputGainLFE.Text = (tbAudioInputGainLFE.Value / 10.0f).ToString("F1");

            ApplyAudioInputGains();
        }

        private void ApplyAudioOutputGains()
        {
            VFAudioEnhancerGains gains = new VFAudioEnhancerGains
            {
                L = tbAudioOutputGainL.Value / 10.0f,
                C = tbAudioOutputGainC.Value / 10.0f,
                R = tbAudioOutputGainR.Value / 10.0f,
                SL = tbAudioOutputGainSL.Value / 10.0f,
                SR = tbAudioOutputGainSR.Value / 10.0f,
                LFE = tbAudioOutputGainLFE.Value / 10.0f
            };

            MediaPlayer1.Audio_Enhancer_OutputGains(-1, gains);
        }

        private void tbAudioOutputGainL_Scroll(object sender, EventArgs e)
        {
            lbAudioOutputGainL.Text = (tbAudioOutputGainL.Value / 10.0f).ToString("F1");

            ApplyAudioOutputGains();
        }

        private void tbAudioOutputGainC_Scroll(object sender, EventArgs e)
        {
            lbAudioOutputGainC.Text = (tbAudioOutputGainC.Value / 10.0f).ToString("F1");

            ApplyAudioOutputGains();
        }

        private void tbAudioOutputGainR_Scroll(object sender, EventArgs e)
        {
            lbAudioOutputGainR.Text = (tbAudioOutputGainR.Value / 10.0f).ToString("F1");

            ApplyAudioOutputGains();
        }

        private void tbAudioOutputGainSL_Scroll(object sender, EventArgs e)
        {
            lbAudioOutputGainSL.Text = (tbAudioOutputGainSL.Value / 10.0f).ToString("F1");

            ApplyAudioOutputGains();
        }

        private void tbAudioOutputGainSR_Scroll(object sender, EventArgs e)
        {
            lbAudioOutputGainSR.Text = (tbAudioOutputGainSR.Value / 10.0f).ToString("F1");

            ApplyAudioOutputGains();
        }

        private void tbAudioOutputGainLFE_Scroll(object sender, EventArgs e)
        {
            lbAudioOutputGainLFE.Text = (tbAudioOutputGainLFE.Value / 10.0f).ToString("F1");

            ApplyAudioOutputGains();
        }

        private void tbAudioTimeshift_Scroll(object sender, EventArgs e)
        {
            lbAudioTimeshift.Text = tbAudioTimeshift.Value.ToString(CultureInfo.InvariantCulture) + " ms";

            MediaPlayer1.Audio_Enhancer_Timeshift(-1, tbAudioTimeshift.Value);
        }

        private void MediaPlayer1_OnLicenseRequired(object sender, LicenseEventArgs e)
        {
            if (cbLicensing.Checked)
            {
                mmLog.Text += "LICENSING:" + Environment.NewLine + e.Message + Environment.NewLine;
            }
        }

        private void btReadTags_Click(object sender, EventArgs e)
        {
            var tags = MediaPlayer1.Tags_Read(edFilenameOrURL.Text);

            if (tags?.Pictures != null)
            {
                if (tags.Pictures.Length > 0)
                {
                    imgTags.Image = tags.Pictures[0];
                }
            }

            edTags.Text = tags?.ToString();            
        }

        private void Form1_Shown(object sender, EventArgs e)
        {

        }

        private void btReversePlaybackPrevFrame_Click(object sender, EventArgs e)
        {
            MediaPlayer1.ReversePlayback_PreviousFrame();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var startInfo = new ProcessStartInfo("explorer.exe", @"https://visioforge-files.s3.amazonaws.com/redists_net/redist_dotnet_vlc_x86.exe");
            Process.Start(startInfo);
        }

        private void btAudioChannelMapperClear_Click(object sender, EventArgs e)
        {
            audioChannelMapperItems.Clear();
            lbAudioChannelMapperRoutes.Items.Clear();
        }

        private void btAudioChannelMapperAddNewRoute_Click(object sender, EventArgs e)
        {
            var item = new AudioChannelMapperItem
            {
                SourceChannel = Convert.ToInt32(this.edAudioChannelMapperSourceChannel.Text),
                TargetChannel = Convert.ToInt32(this.edAudioChannelMapperTargetChannel.Text),
                TargetChannelVolume = this.tbAudioChannelMapperVolume.Value / 1000.0f
            };

            audioChannelMapperItems.Add(item);

            lbAudioChannelMapperRoutes.Items.Add(
                "Source: " + edAudioChannelMapperSourceChannel.Text + ", target: "
                + edAudioChannelMapperTargetChannel.Text + ", volume: "
                + (tbAudioChannelMapperVolume.Value / 1000.0f).ToString("F2"));
        }

        private void tbGPULightness_Scroll(object sender, EventArgs e)
        {
            IVFGPUVideoEffectBrightness intf;
            var effect = MediaPlayer1.Video_Effects_GPU_Get("Brightness");
            if (effect == null)
            {
                intf = new VFGPUVideoEffectBrightness(true, tbGPULightness.Value);
                MediaPlayer1.Video_Effects_GPU_Add(intf);
            }
            else
            {
                intf = effect as IVFGPUVideoEffectBrightness;
                if (intf != null)
                {
                    intf.Value = tbGPULightness.Value;
                    intf.Update();
                }
            }
        }

        private void tbGPUSaturation_Scroll(object sender, EventArgs e)
        {
            IVFGPUVideoEffectSaturation intf;
            var effect = MediaPlayer1.Video_Effects_GPU_Get("Saturation");
            if (effect == null)
            {
                intf = new VFGPUVideoEffectSaturation(true, tbGPUSaturation.Value);
                MediaPlayer1.Video_Effects_GPU_Add(intf);
            }
            else
            {
                intf = effect as IVFGPUVideoEffectSaturation;
                if (intf != null)
                {
                    intf.Value = tbGPUSaturation.Value;
                    intf.Update();
                }
            }
        }

        private void tbGPUContrast_Scroll(object sender, EventArgs e)
        {
            IVFGPUVideoEffectContrast intf;
            var effect = MediaPlayer1.Video_Effects_GPU_Get("Contrast");
            if (effect == null)
            {
                intf = new VFGPUVideoEffectContrast(true, tbGPUContrast.Value);
                MediaPlayer1.Video_Effects_GPU_Add(intf);
            }
            else
            {
                intf = effect as VFGPUVideoEffectContrast;
                if (intf != null)
                {
                    intf.Value = tbGPUContrast.Value;
                    intf.Update();
                }
            }
        }

        private void tbGPUDarkness_Scroll(object sender, EventArgs e)
        {
            IVFGPUVideoEffectDarkness intf;
            var effect = MediaPlayer1.Video_Effects_GPU_Get("Darkness");
            if (effect == null)
            {
                intf = new VFGPUVideoEffectDarkness(true, tbGPUDarkness.Value);
                MediaPlayer1.Video_Effects_GPU_Add(intf);
            }
            else
            {
                intf = effect as IVFGPUVideoEffectDarkness;
                if (intf != null)
                {
                    intf.Value = tbGPUDarkness.Value;
                    intf.Update();
                }
            }
        }

        private void cbGPUGreyscale_CheckedChanged(object sender, EventArgs e)
        {
            IVFGPUVideoEffectGrayscale intf;
            var effect = MediaPlayer1.Video_Effects_GPU_Get("Grayscale");
            if (effect == null)
            {
                intf = new VFGPUVideoEffectGrayscale(cbGPUGreyscale.Checked);
                MediaPlayer1.Video_Effects_GPU_Add(intf);
            }
            else
            {
                intf = effect as IVFGPUVideoEffectGrayscale;
                if (intf != null)
                {
                    intf.Enabled = cbGPUGreyscale.Checked;
                    intf.Update();
                }
            }
        }

        private void cbGPUInvert_CheckedChanged(object sender, EventArgs e)
        {
            IVFGPUVideoEffectInvert intf;
            var effect = MediaPlayer1.Video_Effects_GPU_Get("Invert");
            if (effect == null)
            {
                intf = new VFGPUVideoEffectInvert(cbGPUInvert.Checked);
                MediaPlayer1.Video_Effects_GPU_Add(intf);
            }
            else
            {
                intf = effect as IVFGPUVideoEffectInvert;
                if (intf != null)
                {
                    intf.Enabled = cbGPUInvert.Checked;
                    intf.Update();
                }
            }
        }

        private void cbGPUNightVision_CheckedChanged(object sender, EventArgs e)
        {
            IVFGPUVideoEffectNightVision intf;
            var effect = MediaPlayer1.Video_Effects_GPU_Get("NightVision");
            if (effect == null)
            {
                intf = new VFGPUVideoEffectNightVision(cbGPUNightVision.Checked);
                MediaPlayer1.Video_Effects_GPU_Add(intf);
            }
            else
            {
                intf = effect as IVFGPUVideoEffectNightVision;
                if (intf != null)
                {
                    intf.Enabled = cbGPUNightVision.Checked;
                    intf.Update();
                }
            }
        }

        private void cbGPUPixelate_CheckedChanged(object sender, EventArgs e)
        {
            IVFGPUVideoEffectPixelate intf;
            var effect = MediaPlayer1.Video_Effects_GPU_Get("Pixelate");
            if (effect == null)
            {
                intf = new VFGPUVideoEffectPixelate(cbGPUPixelate.Checked);
                MediaPlayer1.Video_Effects_GPU_Add(intf);
            }
            else
            {
                intf = effect as IVFGPUVideoEffectPixelate;
                if (intf != null)
                {
                    intf.Enabled = cbGPUPixelate.Checked;
                    intf.Update();
                }
            }
        }

        private void cbGPUDenoise_CheckedChanged(object sender, EventArgs e)
        {
            IVFGPUVideoEffectDenoise intf;
            var effect = MediaPlayer1.Video_Effects_GPU_Get("Denoise");
            if (effect == null)
            {
                intf = new VFGPUVideoEffectDenoise(cbGPUDenoise.Checked);
                MediaPlayer1.Video_Effects_GPU_Add(intf);
            }
            else
            {
                intf = effect as IVFGPUVideoEffectDenoise;
                if (intf != null)
                {
                    intf.Enabled = cbGPUDenoise.Checked;
                    intf.Update();
                }
            }
        }

        private void cbGPUDeinterlace_CheckedChanged(object sender, EventArgs e)
        {
            IVFGPUVideoEffectDeinterlaceBlend intf;
            var effect = MediaPlayer1.Video_Effects_GPU_Get("DeinterlaceBlend");
            if (effect == null)
            {
                intf = new VFGPUVideoEffectDeinterlaceBlend(cbGPUDeinterlace.Checked);
                MediaPlayer1.Video_Effects_GPU_Add(intf);
            }
            else
            {
                intf = effect as IVFGPUVideoEffectDeinterlaceBlend;
                if (intf != null)
                {
                    intf.Enabled = cbGPUDeinterlace.Checked;
                    intf.Update();
                }
            }
        }

        private void cbGPUBlur_CheckedChanged(object sender, EventArgs e)
        {
            IVFGPUVideoEffectBlur intf;
            var effect = MediaPlayer1.Video_Effects_GPU_Get("Blur");
            if (effect == null)
            {
                intf = new VFGPUVideoEffectBlur(cbGPUBlur.Checked, 50);
                MediaPlayer1.Video_Effects_GPU_Add(intf);
            }
            else
            {
                intf = effect as IVFGPUVideoEffectBlur;
                if (intf != null)
                {
                    intf.Enabled = cbGPUBlur.Checked;
                    intf.Update();
                }
            }
        }

        private void cbGPUOldMovie_CheckedChanged(object sender, EventArgs e)
        {
            IVFGPUVideoEffectOldMovie intf;
            var effect = MediaPlayer1.Video_Effects_GPU_Get("OldMovie");
            if (effect == null)
            {
                intf = new VFGPUVideoEffectOldMovie(cbGPUOldMovie.Checked);
                MediaPlayer1.Video_Effects_GPU_Add(intf);
            }
            else
            {
                intf = effect as IVFGPUVideoEffectOldMovie;
                if (intf != null)
                {
                    intf.Enabled = cbGPUOldMovie.Checked;
                    intf.Update();
                }
            }
        }

        private void btReversePlaybackNextFrame_Click(object sender, EventArgs e)
        {
            MediaPlayer1.ReversePlayback_NextFrame();
        }

        private void btPreviousFrame_Click(object sender, EventArgs e)
        {
            MediaPlayer1.PreviousFrame();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text += " (SDK v" + MediaPlayer1.SDK_Version.ToString() + ", " + MediaPlayer1.SDK_State + "), VB.Net";

            // set combobox indexes
            cbSourceMode.SelectedIndex = 0;
            cbImageType.SelectedIndex = 1;
            cbTextLogoAlign.SelectedIndex = 0;
            cbTextLogoAntialiasing.SelectedIndex = 0;
            cbTextLogoDrawMode.SelectedIndex = 0;
            cbTextLogoEffectrMode.SelectedIndex = 0;
            cbTextLogoGradMode.SelectedIndex = 0;
            cbTextLogoShapeType.SelectedIndex = 0;
            cbMotDetHLColor.SelectedIndex = 1;
            cbBarcodeType.SelectedIndex = 0;
            cbDirect2DRotate.SelectedIndex = 0;

            rbMotionDetectionExProcessor.SelectedIndex = 1;
            rbMotionDetectionExDetector.SelectedIndex = 1;

            foreach (var device in MediaPlayer1.Audio_OutputDevices)
            {
                cbAudioOutputDevice.Items.Add(device);
            }
            
            if (cbAudioOutputDevice.Items.Count > 0)
            {
                cbAudioOutputDevice.SelectedIndex = 0;
            }

            MediaInfo.ReadFilters();

            foreach (var filter in MediaInfo.List_DirectShowFilters())
            {
                cbCustomVideoDecoder.Items.Add(filter);
                cbCustomAudioDecoder.Items.Add(filter);
                cbCustomSplitter.Items.Add(filter);
                cbFilters.Items.Add(filter);
            }

            if (cbFilters.Items.Count > 0)
            {
                cbFilters.SelectedIndex = 0;
                cbFilters_SelectedIndexChanged(null, null);
            }

            rbEVR.Enabled = MediaPlayer1.Filter_Supported_EVR();
            rbVMR9.Enabled = MediaPlayer1.Filter_Supported_VMR9();

            if (rbEVR.Enabled)
            {
                rbEVR.Checked = true;
            }
            else if (!rbVMR9.Enabled)

            {
                rbVR.Checked = true;
            }

            rbVR_CheckedChanged(sender, e);

            // ReSharper disable once CoVariantArrayConversion
            cbAudEqualizerPreset.Items.AddRange(MediaPlayer1.Audio_Effects_Equalizer_Presets().ToArray());
            cbAudEqualizerPreset.SelectedIndex = 0;

            edScreenshotsFolder.Text =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\VisioForge\\";
            MediaPlayer1.Debug_Dir =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\VisioForge\\";
        }
    }
}

// ReSharper restore InconsistentNaming