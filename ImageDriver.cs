using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Windows.Forms;

namespace ImageDriver
{
    public class OpenCV
	{
		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_TERM();

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_INITILIZE(int width, int height, int id);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_GRAY_THRESH_MERGE(int threshMin, int threshMax);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_TERM_MERGE_IMAGE();

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_RESET_MASK(Int32 x, Int32 y, Int32 w, Int32 h);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_RESET(Int32 wid, Int32 hei);//, Int32 mx, Int32 my, Int32 mw, Int32 mh);

		[DllImport("IMGSUB.DLL")]
		public static extern Int32 OCV_SET_IMG(IntPtr ptr, int wid, int hei, int str, int bpp);

		[DllImport("IMGSUB.DLL")]
		public static extern Int32 OCV_MEM_CPY(IntPtr desptr, IntPtr srcptr, int size);

		[DllImport("IMGSUB.DLL")]
		public static extern Int32 OCV_GET_IMG(IntPtr ptr, int wid, int hei, int str, int bpp);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_TO_GRAY(Int32 I, Int32 H);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_TO_HSV(Int32 I, Int32 H);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_MERGE(Int32 H1, Int32 H2, Int32 H3, Int32 I);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_SPLIT(Int32 I, Int32 H1, Int32 H2, Int32 H3);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_SMOOTH(Int32 I, Int32 cof);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_THRESH_BIN(Int32 I, Int32 H, Int32 thval, Int32 inv);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_THRESH_RANGE(Int32 I, Int32 H, Int32 thval, Int32 thmax, Int32 inv);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_THRESH_HSV(Int32 I1, Int32 I2, Int32 I3, Int32 H, Int32 minh, Int32 maxh, Int32 mins, Int32 maxs, Int32 minv, Int32 maxv);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_CAL_HIST(Int32 I, Int32 bMASK, ref double pval, out double pmin, out double pmax, out double pavg);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_PUTTEXT(Int32 I, string buf, Int32 x, Int32 y, Int32 c);

		/*
		抽出モード:
				0:CV_RETR_EXTERNAL:最も外側の輪郭のみ抽出
				1:CV_RETR_LIST	:全ての輪郭を抽出し，リストに追加
				2:CV_RETR_CCOMP	:全ての輪郭を抽出し，二つのレベルを持つ階層構造を構成する．
								:1番目のレベルは連結成分の外側の境界線，
								:2番目のレベルは穴（連結成分の内側に存在する）の境界線．
				3:CV_RETR_TREE	:全ての輪郭を抽出し，枝分かれした輪郭を完全に表現する階層構造を構成する．
		 */
		[DllImport("IMGSUB.DLL")]
//		public static extern void OCV_FIND_FIRST(Int32 I, Int32 mode);
		public static extern IntPtr OCV_FIND_FIRST(Int32 I, out int numContour);

		[DllImport("IMGSUB.DLL")]
		public static extern IntPtr OCV_FIND_NEXT(IntPtr pos, Int32 smax, Int32 smin, Int32 lmax, Int32 lmin, double cmax, double cmin, out double ps, out double pl, out double pc, out IntPtr startpos);

        [DllImport("IMGSUB.DLL")]
        public static extern IntPtr OCV_FIND_NEXT_ONE(IntPtr pos, out double ps, out double pl, out double pc, out int gX, out int gY);

        [DllImport("IMGSUB.DLL")]
        public static extern IntPtr OCV_FIND_NEXT_ALL(IntPtr pos, Int32 smax, Int32 smin, Int32 lmax, Int32 lmin, double cmax, double cmin, out double ps, out double pl, out double pc, out int gX, out int gY, ref int count, int flg);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_FIND_TERM();

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_DRAW_CONTOURS(Int32 I, IntPtr pos, Int32 c1, Int32 c2);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_DRAW_CONTOURS2(Int32 I, IntPtr pos, Int32 c1, Int32 c2, Int32 thickness);

		[DllImport("IMGSUB.DLL")]
		public static extern Int32 OCV_CONTOURS_CNT(IntPtr pos);
		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_CONTOURS_PTS(IntPtr pos, Int32 idx, out POINT p);
		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_FIT_LINE(IntPtr pos, out float f);

		[DllImport("IMGSUB.DLL")]
		public static extern IntPtr OCV_APPROX_PTS_XY(IntPtr pos, Int32 bSIGNE, Int32 PREC, out int n, ref int px, ref int py);

		[DllImport("IMGSUB.DLL")]
		public static extern Int32 OCV_APPROX_PTS(IntPtr pos, Int32 bSIGNE, Int32 PREC);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_GET_PTS(int idx, out POINT p);
		public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
		public struct POINT { public int x; public int y;}

		[DllImport("IMGSUB.DLL")]
		public static extern void GET_SPLINE_DATA(Int32 n, ref int x, ref double y, int xx, out double spl_data);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_DRAW_LINE(int I, ref POINT p1, ref POINT p2, Int32 c, Int32 thick);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_DRAW_RECT(int I, ref RECT pr, Int32 c, Int32 thickness);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_BOUNDING_RECT(IntPtr pos, out RECT pr);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_DRAW_TEXT(int I, int x, int y, string buf, Int32 c);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_MIN_AREA_RECT2(IntPtr pos, out POINT p1, out POINT p2, out POINT p3, out POINT p4);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_FILL_POLY(int I, ref POINT p, Int32 n, Int32 c);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_ZERO(int I);
		
		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_SOBEL(Int32 I, Int32 H, Int32 xorder, Int32 yorder, Int32 apert_size);
		
		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_LAPLACE(Int32 I, Int32 H, Int32 apert_size);
		
		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_CANNY(Int32 I, Int32 H, double th1, double th2, Int32 apert_size);
		
		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_MINMAX(Int32 I, ref double pmin, ref double pmax);
		
		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_SCALE(Int32 I, Int32 H, double scale, double shift);
		
		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_SMOOTH2(Int32 I, Int32 cof, double sig1, double sig2);
		
		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_DIFF(Int32 I, Int32 H, Int32 J);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_TO_01(Int32 I, Int32 ZERO_VAL, Int32 NONZERO_VAL);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_THINNING(Int32 I, Int32 H, Int32 cnt);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_COPY(Int32 I, Int32 H);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_NOT(Int32 I, Int32 H);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_ERODE(Int32 I, Int32 H, Int32 kernel_size, Int32 cnt);

		[DllImport("IMGSUB.DLL")]
		public static extern void OCV_DILATE(Int32 I, Int32 H, Int32 kernel_size, Int32 cnt);
	}
}
