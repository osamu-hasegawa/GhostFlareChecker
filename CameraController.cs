using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Reflection;
using System.Collections;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;

using CameraDriver;
using ImageDriver;

namespace GhostFlareChecker
{
    public class CameraController
    {
        static public CArtCam m_CArtCam;
		private byte[] m_pCapture;

		public byte[] m_pWorkBuffer;
		Queue<byte[]> queue = new Queue<byte[]>();

//		private Bitmap m_Bitmap = null;
		IntPtr imagehandle;
		IntPtr callbackhandle;
		int width;
		int height;
		private CAMERAINFO Info;
		int isResult;

        public CameraController()
        {
            m_CArtCam = new CArtCam();
        }

		public void LoadSetting()
		{
//���݂̐ݒ���擾
			int lHT;
			int lHS;
			int lHE;
			int lVT;
			int lVS;
			int lVE;
			m_CArtCam.GetCaptureWindowEx(out lHT, out lHS, out lHE, out lVT, out lVS, out lVE);

			DataController.SETDATA.MonitorColorMode = m_CArtCam.GetColorMode();
            DataController.SETDATA.cameraType = m_CArtCam.GetCameraType();
            DataController.SETDATA.width = m_CArtCam.Width();
            DataController.SETDATA.height = m_CArtCam.Height();
            DataController.SETDATA.Fps = m_CArtCam.Fps() / 10;//�擾�l��1/10(�P�ʁFx/second)
			DataController.SETDATA.dllVersion = m_CArtCam.GetDllVersion();

            byte byteData;
            int longData;
            int Reserve = 0;
            bool isResult = m_CArtCam.GetIOPort(out byteData, out longData, Reserve);

            SUBSAMPLE subSample = m_CArtCam.GetSubSample();

            DataController.SETDATA.waitTime = m_CArtCam.GetWaitTime();
            DataController.SETDATA.isMirrorV = m_CArtCam.GetMirrorV();
            DataController.SETDATA.isMirrorH = m_CArtCam.GetMirrorH();
            DataController.SETDATA.brightness = m_CArtCam.GetBrightness();
            DataController.SETDATA.contrast = m_CArtCam.GetContrast();
            DataController.SETDATA.hue = m_CArtCam.GetHue();
            DataController.SETDATA.saturation = m_CArtCam.GetSaturation();

            DataController.SETDATA.sharpness = m_CArtCam.GetSharpness();

			DataController.SETDATA.bayerGainRed = m_CArtCam.GetBayerGainRed();
			DataController.SETDATA.bayerGainGreen = m_CArtCam.GetBayerGainGreen();
			DataController.SETDATA.bayerGainBlue = m_CArtCam.GetBayerGainBlue();

            int auto = m_CArtCam.GetBayerGainAuto();
            if(auto == 0)
            {
                DataController.SETDATA.auto = true;
            }else
            {
                DataController.SETDATA.auto = false;
			}

            DataController.SETDATA.gamma = m_CArtCam.GetGamma();

            DataController.SETDATA.globalGain = m_CArtCam.GetGlobalGain();
            DataController.SETDATA.gainRed = m_CArtCam.GetColorGainRed();
            DataController.SETDATA.gainGreen1 = m_CArtCam.GetColorGainGreen1();
            DataController.SETDATA.gainGreen2 = m_CArtCam.GetColorGainGreen2();
            DataController.SETDATA.colorGainBlue = m_CArtCam.GetColorGainBlue();
            DataController.SETDATA.exposureTime = m_CArtCam.GetExposureTime();
            DataController.SETDATA.halfClock = m_CArtCam.GetHalfClock();

            AI_TYPE aiType = m_CArtCam.GetAutoIris();
            SAMPLING_RATE sampleRate = m_CArtCam.GetSamplingRate();
            VIDEOFORMAT videoInfo = m_CArtCam.GetVideoFormat();

            int writeAddress = 0;
            int Value = 0;
            int writeSromID = m_CArtCam.WriteSromID(writeAddress, Value);

            int readAddress = 0;
            int readSlomID = m_CArtCam.ReadSromID(readAddress);

			DataController.SETDATA.cameraInfo = m_CArtCam.GetCameraInfo(ref Info);
            DataController.SETDATA.cameraStatus = m_CArtCam.GetStatus();

            int writeRegister = m_CArtCam.WriteRegister(writeAddress, Value);

            int readRegister = m_CArtCam.ReadRegister(readAddress);
            int grayMode = m_CArtCam.GetGrayMode();
            int grayGrayGainR = m_CArtCam.GetGrayGainR();
            int grayGrayGainG1 = m_CArtCam.GetGrayGainG1();
            int grayGrayGainG2 = m_CArtCam.GetGrayGainG2();
            int grayGrayGainB = m_CArtCam.GetGrayGainB();
		}

        public void Init()
        {
			Release();
			m_CArtCam.FreeLibrary();

#if true
			if(!m_CArtCam.LoadLibrary("ArtCamSdk_178IMX_USB3_T2.dll"))
#else
			if(!m_CArtCam.LoadLibrary("ArtCamSdk_178IMX_USB3_T2_x64.dll"))//error���Ԃ� Win32Library()�Ŕ��f���Ă����
#endif
			{
				MessageBox.Show("DLL��������܂���\n");
				return;
			}

			// Initialize �͍ŏ��ɌĂт܂�
			// �����ŃE�B���h�E�n���h����ݒ肷���WM_ERROR���擾���鎖���o����
			if (!m_CArtCam.Initialize(CheckerManager.form1.Handle)){
				MessageBox.Show("�r�c�j�̏������Ɏ��s���܂���");
				return;
			}

#if true
			m_CArtCam.SetCaptureWindow(m_CArtCam.Width(), m_CArtCam.Height(), m_CArtCam.Fps());
#else
			m_CArtCam.SetCaptureWindow(m_CArtCam.Width(), m_CArtCam.Height(), 100);//��3������FPS(���ۂ�x10���w��)������������Ă��邩��
#endif
			//�����I��ColorMode��24�ɐݒ肷��(Artray��)
			m_CArtCam.SetColorMode(24);
        }

        public void Terminate()
		{
            Release();
			m_CArtCam.FreeLibrary();
		}
	
		private void Release()
		{
			isResult = m_CArtCam.Close();
			isResult = m_CArtCam.Release();

//			if(m_Bitmap != null)
//			{
//				m_Bitmap.Dispose();
//				m_Bitmap = null;
//			}
		}

		public void Save_Click(object sender, System.EventArgs e)
		{
			if(!m_CArtCam.IsInit())
			{
				MessageBox.Show("�L���ȃf�o�C�X��I�����Ă�������");
				return;
			}

			SaveImageFile();
		}


		// �v���r���[�@�S�����ŕ`�悷��
		public void Preview_Click(object sender, System.EventArgs e)
		{
			if(!m_CArtCam.IsInit())
			{
				MessageBox.Show("�L���ȃf�o�C�X��I�����Ă�������");
				return;
			}

			// �����ň�U�f�o�C�X���J������
			isResult = m_CArtCam.Close();

            // �\������E�B���h�E��ݒ肷��
            // hWnd ��NULL��ݒ肷��ƐV�����E�B���h�E���쐬���ĕ\�����܂�
			CheckerManager.form2.GetPreviewWindow(out imagehandle, out width, out height);
            isResult = m_CArtCam.SetPreviewWindow(imagehandle, 0, 0, width, height);

            isResult = m_CArtCam.Preview();
		}

		// �R�[���o�b�N�@�f���̃C���[�W�|�C���^���擾���Ď��O�ŕ`�悷��
		public void Callback_Click(object sender, System.EventArgs e, bool flag)
		{
			if(!m_CArtCam.IsInit())
			{
				MessageBox.Show("�L���ȃf�o�C�X��I�����Ă�������");
				return;
			}

			// �����ň�U�f�o�C�X���J������
			isResult = m_CArtCam.Close();

			// �����ŕ`�悷��ꍇ�̓E�B���h�E�T�C�Y��S�ĂO�ɂ��܂�
			// CallBackPreview ���g�p����ꍇ�ł��E�B���h�E�T�C�Y��ݒ肷��Ύ����\���͍s���܂��B
			CheckerManager.form2.GetPreviewWindow(out imagehandle, out width, out height);

			if(flag == true)
			{
	            isResult = m_CArtCam.SetPreviewWindow(imagehandle, 0, 0, width, height);
			}
			else
			{
				//Preview�E�B���h�E��\�����Ȃ��ꍇ
	            isResult = m_CArtCam.SetPreviewWindow((IntPtr)0, 0, 0, 0, 0);
	            CheckerManager.form2.Enabled = false;
	            CheckerManager.form2.Visible = false;
	        }

			// �r�b�g�}�b�v�쐬
			CreateBitmap();

            // �f������荞��
#if false
			isResult = m_CArtCam.CallBackPreview(imagehandle, m_pCapture, getSize(), 1);
#else
			CheckerManager.form1.GetPreviewWindow(out callbackhandle);
			isResult = m_CArtCam.CallBackPreview(callbackhandle, m_pCapture, getSize(), 1);
#endif
		}

		// �r�b�g�}�b�v�쐬
		private void CreateBitmap()
		{
			// ���Ƀr�b�g�}�b�v������Ă���ꍇ�͈�U�J��
//			if(null != m_Bitmap)
//			{
//				m_Bitmap.Dispose();
//			}
//
//			switch(getColorMode())
//			{
//				case 8:
//				case 16:
//					m_Bitmap = new Bitmap(getWidth(), getHeight(), PixelFormat.Format8bppIndexed);
//
//					// �p���b�g�ύX
//					ColorPalette pal = m_Bitmap.Palette;
//					Color[] cpe = m_Bitmap.Palette.Entries;
//
//					for(int i=0; i<256; i++)
//					{
//						cpe.SetValue(Color.FromArgb(i, i, i), i);
//						pal.Entries[i] = cpe[i];
//					}
//					m_Bitmap.Palette = pal;
//				break;
//
//				case 24:	m_Bitmap = new Bitmap(getWidth(), getHeight(), PixelFormat.Format24bppRgb);			break;
//				case 32:	m_Bitmap = new Bitmap(getWidth(), getHeight(), PixelFormat.Format24bppRgb);			break;
//				case 48:	m_Bitmap = new Bitmap(getWidth(), getHeight(), PixelFormat.Format24bppRgb);			break;
//				case 64:	m_Bitmap = new Bitmap(getWidth(), getHeight(), PixelFormat.Format24bppRgb);			break;
//			}

			// �L���v�`���p�̔z��
			if(null == m_pCapture)
			{
				m_pCapture = new Byte[getSize()];
			}
		}

		private int getSize()
		{
#if true
			int size = ((getWidth() * (getColorMode() / 8) + 3) & ~3) * getHeight();
			return ((getWidth() * (getColorMode() / 8) + 3) & ~3) * getHeight();
#else
			return (getWidth() * getHeight());
#endif
		}

		private int getWidth()
		{
#if false
			int[] Size = { 1, 2, 4, 8 };
			return m_CArtCam.Width() / Size[(int)(getSubSample())];
#else
			return DataController.SETDATA.width;
//			return m_CArtCam.Width();
#endif
		}

		private int getHeight()
		{
#if false
			int[] Size = { 1, 2, 4, 8 };
			return m_CArtCam.Height() / Size[(int)getSubSample()];
#else
			return DataController.SETDATA.height;
//			return m_CArtCam.Height();
#endif
		}

		private int getColorMode()
		{
#if false
			return ((m_CArtCam.GetColorMode() + 7) & ~7);
#else
			return 24;
#endif
		}

		private int getSubSample()
		{
#if false
			return ((int)m_CArtCam.GetSubSample() & 0x03);
#else
			return 0;
#endif
		}

//		private BitmapData LockBitmap()
//		{
//			switch(getColorMode())
//			{
//				case 8:
//				case 16:
//					return m_Bitmap.LockBits(new Rectangle(0,0,getWidth(),getHeight()), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
//
//				case 24:
//				case 32:
//				case 48:
//				case 64:
//					return m_Bitmap.LockBits(new Rectangle(0,0,getWidth(),getHeight()), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
//			}
//
//			return null;
//		}

#if false
		public void DrawImage(Graphics g)
		{
			if(null == m_Bitmap)
			{
				return ;
			}

			BitmapData pBitmapData = LockBitmap();
			if(null == pBitmapData)
			{
				return ;
			}

            byte* pdest = (byte*)pBitmapData.Scan0.ToPointer();

			// 16�r�b�g�]���̏ꍇ�͂W�r�b�g�ɕϊ����ĕ\������
			if(getColorMode() == 16)
			{
				int size = getWidth() * getHeight();
				switch(m_CArtCam.GetColorMode())
				{
					case 10: for(int i=0; i<size; i++) pdest[i] = (byte)((m_pCapture[i*2+1] << 6) | (m_pCapture[i*2] >> 2));	break;
					case 12: for(int i=0; i<size; i++) pdest[i] = (byte)((m_pCapture[i*2+1] << 4) | (m_pCapture[i*2] >> 4));	break;
					case 14: for(int i=0; i<size; i++) pdest[i] = (byte)((m_pCapture[i*2+1] << 2) | (m_pCapture[i*2] >> 6));	break;
					case 16: for(int i=0; i<size; i++) pdest[i] = (byte)((m_pCapture[i*2+1] << 0) | (m_pCapture[i*2] >> 8));	break;
				}
			}
			else if(getColorMode() == 32)
			{
				int size = getWidth() * getHeight();
				for(int i=0; i<size; i++)
				{
					pdest[i*3]   = m_pCapture[i*4];
					pdest[i*3+1] = m_pCapture[i*4+1];
					pdest[i*3+2] = m_pCapture[i*4+2];
				}
			}
			// �d���̂Œ��Ӂi��{�I��16(10)�r�b�g�̃J���[�������ۂ͂b�n��̌�����g�p���Ă��������j
			else if(getColorMode() == 48 || getColorMode() == 64)
			{
				int bpp = getColorMode() / 8;
				int size = getWidth() * getHeight();
				switch(m_CArtCam.GetColorMode())
				{
					case 42: case 58:
						for(int i=0; i<size; i++)
						{
							pdest[i*3]   = (byte)((m_pCapture[i*bpp+1] << 6) | (m_pCapture[i*bpp]   >> 2));
							pdest[i*3+1] = (byte)((m_pCapture[i*bpp+3] << 6) | (m_pCapture[i*bpp+2] >> 2));
							pdest[i*3+2] = (byte)((m_pCapture[i*bpp+5] << 6) | (m_pCapture[i*bpp+4] >> 2));
						}
						break;
					case 44: case 60:
						for(int i=0; i<size; i++)
						{
							pdest[i*3]   = (byte)((m_pCapture[i*bpp+1] << 4) | (m_pCapture[i*bpp]   >> 4));
							pdest[i*3+1] = (byte)((m_pCapture[i*bpp+3] << 4) | (m_pCapture[i*bpp+2] >> 4));
							pdest[i*3+2] = (byte)((m_pCapture[i*bpp+5] << 4) | (m_pCapture[i*bpp+4] >> 4));
						}
						break;
					case 46: case 62:
						for(int i=0; i<size; i++)
						{
							pdest[i*3]   = (byte)((m_pCapture[i*bpp+1] << 2) | (m_pCapture[i*bpp]   >> 6));
							pdest[i*3+1] = (byte)((m_pCapture[i*bpp+3] << 2) | (m_pCapture[i*bpp+2] >> 6));
							pdest[i*3+2] = (byte)((m_pCapture[i*bpp+5] << 2) | (m_pCapture[i*bpp+4] >> 6));
						}
						break;
					case 48: case 64:
						for(int i=0; i<size; i++)
						{
							pdest[i*3]   = (byte)((m_pCapture[i*bpp+1] << 0) | (m_pCapture[i*bpp]   >> 8));
							pdest[i*3+1] = (byte)((m_pCapture[i*bpp+3] << 0) | (m_pCapture[i*bpp+2] >> 8));
							pdest[i*3+2] = (byte)((m_pCapture[i*bpp+5] << 0) | (m_pCapture[i*bpp+4] >> 8));
						}
						break;
				}
			}
			// ����ȊO�͂��̂܂܃R�s�[
			else
			{
				int size = 
				getSize();
				for(int i=0; i<size; i++)
				{
					pdest[i] = m_pCapture[i];
				}
			}

            m_Bitmap.UnlockBits(pBitmapData);

			// �f���\��
			int iWidth = m_Bitmap.Width;
			int iHeight= m_Bitmap.Height;
			g.DrawImage(m_Bitmap, new Rectangle(0, 0, getWidth(), getHeight()));
		}
#endif
		
		public void StartPreview()
		{
			isResult = m_CArtCam.StartPreview();
		}

		public void StopPreview()
		{
			isResult = m_CArtCam.StopPreview();
		}


		public void SaveImageFile()//Preview���Stop���Ɏg�p
		{
			string fileName = CheckerManager.m_DataController.GetImageFileName(1);
			m_CArtCam.SaveImage(fileName, FILETYPE.FILETYPE_PNG);
        }

		public string SaveImageFile(Bitmap bmp, int sign)
		{
			string fileName = CheckerManager.m_DataController.GetImageFileName(sign);
			bmp.Save(fileName, ImageFormat.Png);
			return fileName;
		}

		public void SetBrightness(int value)
		{
			m_CArtCam.SetBrightness(value);
		}

		public void SetContrast(int value)
		{
			m_CArtCam.SetContrast(value);
		}

		public void SetHue(int value)
		{
			m_CArtCam.SetHue(value);
		}

		public void SetSaturation(int value)
		{
			m_CArtCam.SetSaturation(value);
		}

		public void SetSharpness(int value)
		{
			m_CArtCam.SetSharpness(value);
		}

		public void SetGamma(int value)
		{
			m_CArtCam.SetGamma(value);
		}

		public void SetGlobalGain(int value)
		{
			m_CArtCam.SetGlobalGain(value);
		}

		public void SetColorGainRed(int value)
		{
			m_CArtCam.SetColorGainRed(value);
		}

		public void SetColorGainGreen1(int value)
		{
			m_CArtCam.SetColorGainGreen1(value);
		}

		public void SetColorGainGreen2(int value)
		{
			m_CArtCam.SetColorGainGreen2(value);
		}

		public void SetColorGainBlue(int value)
		{
			m_CArtCam.SetColorGainBlue(value);
		}

		public void SetExposureTime(int value)
		{
			m_CArtCam.SetExposureTime(value);
		}

		public void SetHalfClock(int value)
		{
			m_CArtCam.SetHalfClock(value);
		}

		public void SetBayerGainRed(int value)
		{
			m_CArtCam.SetBayerGainRed(value);
		}

		public void SetBayerGainGreen(int value)
		{
			m_CArtCam.SetBayerGainGreen(value);
		}

		public void SetBayerGainBlue(int value)
		{
			m_CArtCam.SetBayerGainBlue(value);
		}

		public void SetBayerGainAuto(bool value)
		{
			m_CArtCam.SetBayerGainAuto(value);
		}

		public Bitmap GetImage()
		{
			Bitmap orgbmp = new Bitmap(getWidth(), getHeight(), PixelFormat.Format24bppRgb);
			Rectangle rect = new Rectangle(0, 0, orgbmp.Width, orgbmp.Height); 
			System.Drawing.Imaging.BitmapData bmpData = 
			    orgbmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, 
			    PixelFormat.Format24bppRgb); 

			// Bitmap�̐擪�A�h���X���擾 
			IntPtr ptr = bmpData.Scan0; 
			// Bitmap�փR�s�[ 

#if true
			Marshal.Copy(m_pCapture, 0, ptr, m_pCapture.Length); 
			orgbmp.UnlockBits(bmpData);
#else
			lock(queue)
			{
			    if(queue.Count > 0)
			    {
					byte[] rcv_buf = queue.Dequeue();
				    Marshal.Copy(rcv_buf, 0, ptr, rcv_buf.Length); 
				}
				else
				{
					return null;
				}
			}

			orgbmp.UnlockBits(bmpData);
#endif

			//Bitmap size �k��
			CheckerManager.form3.GetPreviewWindow(out width, out height);
			int resizeWidth = width;
			int resizeHeight = height;

			Bitmap m_bmpR = new Bitmap(resizeWidth, resizeHeight);

			Graphics g = Graphics.FromImage(m_bmpR);
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			g.DrawImage(orgbmp, 0, 0, resizeWidth, resizeHeight);
			orgbmp.Dispose();
			g.Dispose();

            Rectangle rtBmp = new Rectangle(0, 0, m_bmpR.Width, m_bmpR.Height);
            Bitmap cloneBmp = m_bmpR.Clone(rtBmp, PixelFormat.Format24bppRgb);
            m_bmpR.Dispose();
            m_bmpR = cloneBmp;

            return m_bmpR;
		}

		public void deviceClose()
		{
			m_CArtCam.Close();
			Init();
		}

		public void SetColorMode(int value)
		{
			m_CArtCam.SetColorMode(value);
		}

		public void SetFps(int value)
		{
			m_CArtCam.SetCaptureWindow(m_CArtCam.Width(), m_CArtCam.Height(), value);
		}

		public int GetFps()
		{
			return m_CArtCam.Fps();
		}

		public void SaveCaptureBuffer()
		{
			Bitmap orgbmp = new Bitmap(DataController.SETDATA.width, DataController.SETDATA.height, PixelFormat.Format24bppRgb);
			Rectangle rect = new Rectangle(0, 0, orgbmp.Width, orgbmp.Height); 
			System.Drawing.Imaging.BitmapData bmpData = 
			    orgbmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, 
			    PixelFormat.Format24bppRgb); 

			// Bitmap�̐擪�A�h���X���擾 
			IntPtr ptr = bmpData.Scan0; 
			// Bitmap�փR�s�[ 
			Marshal.Copy(m_pCapture, 0, ptr, m_pCapture.Length); 
			orgbmp.UnlockBits(bmpData);


			string pathStr = CheckerManager.m_DataController.GetImageFileName(0);
			orgbmp.Save(pathStr, System.Drawing.Imaging.ImageFormat.Png);
		}

		public void CopyCaptureBuffer()
		{
			Thread.Sleep(20);

			lock(queue)
			{
				m_pWorkBuffer = new Byte[getSize()];
				Array.Copy(m_pCapture, m_pWorkBuffer, m_pCapture.Length);
	            queue.Enqueue(m_pWorkBuffer);
	        }

		}



	}
}
