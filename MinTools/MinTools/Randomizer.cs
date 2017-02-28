using System;

namespace MinTools
{
	public interface IRandomizer
	{
		void LockRandom (bool isLocked, int key = 0);
		void SeedRandom (bool isSeeded, int seed = 0);
		void FixRandom (bool isFixed, float value = -1f);
		bool IsSeeded { get; }
		bool IsFixed { get; }
		bool IsLocked { get; }
		int Seed { get; }
		float Value { get; }
		int GetValue (int maxValue);
		int GetValue (int minValue, int maxValue);
		float GetValue (float maxValue);
		float GetValue (float minValue, float maxValue);
	}

	public class Randomizer : BaseRandomizer
	{
		Random random = new Random();

		protected override void InitRandom ()
		{
			random = new Random();
		}

		protected override void InitSeed (int seed)
		{
			random = new Random(seed);
		}

		protected override float GetNext ()
		{
			return (float)random.NextDouble();
		}
	}

	public abstract class BaseRandomizer : IRandomizer
	{
		float fixedRandom;
		int lockKey;

		public void LockRandom (bool isLocked, int key = 0)
		{
			if (IsLocked != isLocked) {
				if (IsLocked) {
					if (lockKey == key) {
						IsLocked = false;
						lockKey = 0;
					}
				}
				else {
					IsLocked = true;
					lockKey = key;
				}
			}
		}

		public void SeedRandom (bool isSeeded, int seed = 0)
		{
			if (!IsLocked) {
				IsSeeded = isSeeded;
				Seed = seed;

				if (IsSeeded) {
					InitSeed(Seed);
				}
				else {
					InitRandom();
				}
			}
		}

		public void FixRandom (bool isFixed, float value = -1f)
		{
			if (!IsLocked) {
				if (isFixed && value < 0f || value >= 1f) {
					throw new Exception("fixed value must be v >= 0.0f && v < 1.0");
				}

				IsFixed = isFixed;
				fixedRandom = value;
			}
		}

		bool isSeeded;

		public bool IsSeeded {
			get {
				return isSeeded && !IsFixed;
			}

			private set {
				isSeeded = value;
			}
		}

		public bool IsFixed { get; private set; }

		public bool IsLocked { get; private set; }

		public int Seed { get; private set; }

		public int GetValue (int maxValue)
		{
			return GetValue(0, maxValue);
		}

		public int GetValue (int minValue, int maxValue)
		{
			return (int)CalcValueInRange(minValue, maxValue);
		}

		public float GetValue (float maxValue)
		{
			return GetValue(0f, maxValue);
		}

		public float GetValue (float minValue, float maxValue)
		{
			return CalcValueInRange(minValue, maxValue);
		}

		float CalcValueInRange (float minValue, float maxValue)
		{
			var diff = maxValue - minValue;
			var result = diff * Value + minValue;

			return result;
		}

		public float Value {
			get {
				return IsFixed ? fixedRandom : GetNext();
			}
		}

		protected abstract float GetNext ();

		protected abstract void InitRandom ();

		protected abstract void InitSeed (int seed);
	}
}
