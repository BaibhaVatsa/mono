using System;
using System.Reflection;

public struct A
{
	public override string ToString ()
	{
		return "A";
	}
}

public class D
{
	public string Test ()
	{
		return "Test";
	}
}

enum Enum1
{
	A,
	B
}

enum Enum2
{
	C,
	D
}

class Tests
{
	public static Enum1 return_enum1 () {
		return Enum1.A;
	}

	public static Enum2 return_enum2 () {
		return Enum2.C;
	}

	public static long return_long () {
		return 1234;
	}

	public static ulong return_ulong () {
		return UInt64.MaxValue - 5;
	}

	static int Main (string[] args)
	{
		return TestDriver.RunTests (typeof (Tests), args);
	}

	public static int test_0_base () {
		Assembly ass = typeof (Tests).Assembly;
		Type a_type = ass.GetType ("A");
		MethodInfo a_method = a_type.GetMethod ("ToString");

		Type d_type = ass.GetType ("D");
		MethodInfo d_method = d_type.GetMethod ("Test");

		Console.WriteLine ("TEST: {0} {1}", a_method, d_method);

		A a = new A ();
		D d = new D ();

		object a_ret = a_method.Invoke (a, null);
		Console.WriteLine (a_ret);

		object d_ret = d_method.Invoke (d, null);
		Console.WriteLine (d_ret);

		return 0;
	}

	public static int test_0_enum_sharing () {
		/* Check sharing of wrappers returning enums */
		if (typeof (Tests).GetMethod ("return_enum1").Invoke (null, null).GetType () != typeof (Enum1))
			return 1;
		if (typeof (Tests).GetMethod ("return_enum2").Invoke (null, null).GetType () != typeof (Enum2))
			return 2;
		return 0;
	}

	public static int test_0_primitive_sharing () {
		/* Check sharing of wrappers returning primitive types */
		if (typeof (Tests).GetMethod ("return_long").Invoke (null, null).GetType () != typeof (long))
			return 3;
		if (typeof (Tests).GetMethod ("return_ulong").Invoke (null, null).GetType () != typeof (ulong))
			return 4;

		return 0;
	}

	public struct Foo
	{
		public string ToString2 () {
			return "FOO";
		}
	}

	public static object GetNSObject (IntPtr i) {
		return i;
	}

	public static int test_0_vtype_method_sharing () {
		/* Check sharing of wrappers of vtype methods with static methods having an IntPtr argument */
		if ((string)(typeof (Foo).GetMethod ("ToString2").Invoke (new Foo (), null)) != "FOO")
			return 3;
		object o = typeof (Tests).GetMethod ("GetNSObject").Invoke (null, new object [] { new IntPtr (42) });
		if (!(o is IntPtr) || ((IntPtr)o != new IntPtr (42)))
			return 4;

		return 0;
	}

	public static unsafe int test_0_ptr () {
		int[] arr = new int [10];
		fixed (void *p = &arr [5]) {
			object o = typeof (Tests).GetMethod ("Test").Invoke (null, new object [1] { new IntPtr (p) });
			void *p2 = Pointer.Unbox (o);
			if (new IntPtr (p) != new IntPtr (p2))
				return 1;

			o = typeof (Tests).GetMethod ("Test").Invoke (null, new object [1] { null });
			p2 = Pointer.Unbox (o);
			if (new IntPtr (p2) != IntPtr.Zero)
				return 1;
		}

		return 0;
	}

    public static unsafe int* Test (int *val) {
		Console.WriteLine (new IntPtr (val));
        return val;
    }
}
