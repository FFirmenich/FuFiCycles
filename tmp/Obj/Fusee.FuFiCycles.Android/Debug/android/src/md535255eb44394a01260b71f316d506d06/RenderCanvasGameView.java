package md535255eb44394a01260b71f316d506d06;


public class RenderCanvasGameView
	extends opentk.platform.android.AndroidGameView
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Fusee.Engine.Imp.Graphics.Android.RenderCanvasGameView, Fusee.Engine.Imp.Graphics.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", RenderCanvasGameView.class, __md_methods);
	}


	public RenderCanvasGameView (android.content.Context p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == RenderCanvasGameView.class)
			mono.android.TypeManager.Activate ("Fusee.Engine.Imp.Graphics.Android.RenderCanvasGameView, Fusee.Engine.Imp.Graphics.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public RenderCanvasGameView (android.content.Context p0, android.util.AttributeSet p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == RenderCanvasGameView.class)
			mono.android.TypeManager.Activate ("Fusee.Engine.Imp.Graphics.Android.RenderCanvasGameView, Fusee.Engine.Imp.Graphics.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}


	public RenderCanvasGameView (android.content.Context p0, android.util.AttributeSet p1, int p2) throws java.lang.Throwable
	{
		super (p0, p1, p2);
		if (getClass () == RenderCanvasGameView.class)
			mono.android.TypeManager.Activate ("Fusee.Engine.Imp.Graphics.Android.RenderCanvasGameView, Fusee.Engine.Imp.Graphics.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public RenderCanvasGameView (android.content.Context p0, android.util.AttributeSet p1, int p2, int p3) throws java.lang.Throwable
	{
		super (p0, p1, p2, p3);
		if (getClass () == RenderCanvasGameView.class)
			mono.android.TypeManager.Activate ("Fusee.Engine.Imp.Graphics.Android.RenderCanvasGameView, Fusee.Engine.Imp.Graphics.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
