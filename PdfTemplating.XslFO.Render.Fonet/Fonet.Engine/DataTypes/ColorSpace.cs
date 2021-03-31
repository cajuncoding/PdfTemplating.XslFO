namespace Fonet.DataTypes
{
    internal class ColorSpace
    {
        public const int DeviceUnknown = -1;
        public const int DeviceGray = 1;
        public const int DeviceRgb = 2;
        public const int DeviceCmyk = 3;

        protected int currentColorSpace = -1;

        private bool hasICCProfile;
        private byte[] iccProfile;
        private int numComponents;

        public ColorSpace(int theColorSpace)
        {
            this.currentColorSpace = theColorSpace;
            this.hasICCProfile = false;
            this.numComponents = this.CalculateNumComponents();
        }

        public void SetColorSpace(int theColorSpace)
        {
            this.currentColorSpace = theColorSpace;
            this.numComponents = this.CalculateNumComponents();
        }

        public bool HasICCProfile()
        {
            return this.hasICCProfile;
        }

        public byte[] GetICCProfile()
        {
            if (this.hasICCProfile)
            {
                return this.iccProfile;
            }
            else
            {
                return new byte[0];
            }
        }

        public void SetICCProfile(byte[] iccProfile)
        {
            this.iccProfile = iccProfile;
            this.hasICCProfile = true;
        }

        public int GetColorSpace()
        {
            return this.currentColorSpace;
        }

        public int GetNumComponents()
        {
            return this.numComponents;
        }

        public string GetColorSpacePDFString()
        {
            if (this.currentColorSpace == DeviceRgb)
            {
                return "DeviceRGB";
            }
            else if (this.currentColorSpace == DeviceCmyk)
            {
                return "DeviceCMYK";
            }
            else if (this.currentColorSpace == DeviceGray)
            {
                return "DeviceGray";
            }
            else
            {
                return "DeviceRGB";
            }
        }

        private int CalculateNumComponents()
        {
            if (this.currentColorSpace == DeviceGray)
            {
                return 1;
            }
            else if (this.currentColorSpace == DeviceRgb)
            {
                return 3;
            }
            else if (this.currentColorSpace == DeviceCmyk)
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }
    }
}