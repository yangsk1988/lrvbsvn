using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VirtualBicycle.Core;
using VirtualBicycle.IO;

namespace VirtualBicycle.Sound
{
    public class SfxType : Resource
    {
        FMOD.Sound fmodSound;
        FMOD.System soundSystem;

        bool isUnloadable;

        ResourceLocation soundLocation;


        public SfxType(SfxManager mgr, ResourceLocation resLoc)
            : base(mgr, resLoc.Name)
        {
            this.soundLocation = resLoc;
            this.soundSystem = mgr.SoundSystem;
            this.Flags = FMOD.MODE.DEFAULT;
        }

        public void Play()
        {
            isUnloadable = true;

        }

        void CreateSound(byte[] data) 
        {
            FMOD.CREATESOUNDEXINFO exinfo = new FMOD.CREATESOUNDEXINFO();
            exinfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(exinfo);
            exinfo.length = (uint)data.Length;

            soundSystem.createSound(data, FMOD.MODE.CREATESAMPLE | FMOD.MODE.LOWMEM | FMOD.MODE._2D | FMOD.MODE.DEFAULT | FMOD.MODE.SOFTWARE | FMOD.MODE.OPENMEMORY | Flags, ref exinfo, ref fmodSound);
        }

        public FMOD.MODE Flags
        {
            get;
            set;
        }

        public override void ReadCacheData(Stream stream)
        {
            ContentBinaryReader br = new ContentBinaryReader(soundLocation);
            byte[] data = br.ReadBytes(soundLocation.Size);
            br.Close();

            CreateSound(data);
        }

        public override void WriteCacheData(Stream stream)
        {
            if (soundLocation != null)
            {
                Stream stm = soundLocation.GetStream;

                int len = (int)stm.Length;

                byte[] buffer = new byte[len];
                stm.Read(buffer, 0, len);
                stream.Write(buffer, 0, len);
            }
            else 
            {
                throw new NotSupportedException();
            }
        }

        public override int GetSize()
        {
            if (soundLocation != null)
            {
                return soundLocation.Size;
            }
            return 0;
        }

        protected override void load()
        {
            ContentBinaryReader br = new ContentBinaryReader(soundLocation);
            byte[] data = br.ReadBytes(soundLocation.Size);
            br.Close();

            CreateSound(data);
        }

        protected override void unload()
        {
            fmodSound.release();
        }

        public override bool IsUnloadable
        {
            get { return isUnloadable; }
        }
    }
}
