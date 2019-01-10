using XLua;
namespace behaviac
{
[Hotfix]
	class RandomGenerator
	{
        private static RandomGenerator Instance = null;
		public static RandomGenerator GetInstance()
		{
            if (Instance == null)
            {
                Instance = new RandomGenerator(0);
            }
			return  RandomGenerator.Instance;
		}

		//[0, 1)
		public float GetRandom()
		{
			m_seed = 214013 * m_seed + 2531011;
			float r = (m_seed * (1.0f / 4294967296.0f));

			Debug.Check(r >= 0.0f && r < 1.0f);
			return r;
		}

		//[low, high)
        public float InRange(float low, float high)
		{
            float r = this.GetRandom();
			float ret = r * (high - low) + low;
			return ret;
		}

        public void SetSeed(uint seed)
		{
			this.m_seed = seed;
		}
	
		protected RandomGenerator(uint seed)
		{
            m_seed = seed;
		}

		~RandomGenerator()
		{}

		private uint m_seed;
	};
}

