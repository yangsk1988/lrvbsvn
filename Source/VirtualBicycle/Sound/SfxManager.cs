using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VirtualBicycle.Config;
using VirtualBicycle.ConfigModel;
using VirtualBicycle.IO;

namespace VirtualBicycle.Sound
{
    public class SfxManager : ResourceManager
    {
        AudioConfigs audioCons;
        FMOD.System sndSys;

        Dictionary<string, SfxType> sfx;

        static SfxManager singleton;

        public static SfxManager Instance 
        {
            get { return singleton; }
        }

        public static void Initialize(FMOD.System sndSys, AudioConfigs aus)
        {
            if (singleton == null)
            {
                singleton = new SfxManager(sndSys, aus);
            }
        }

        private SfxManager(FMOD.System sndSys, AudioConfigs aus)
            : base(8 * 1048576)
        {
            this.sndSys = sndSys;
            this.audioCons = aus;

            this.sfx = new Dictionary<string, SfxType>();

            LoadAllSounds(); 
        }

        /// <summary>
        /// 加载所有声音
        /// </summary>
        void LoadAllSounds()
        {
            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(Paths.Configs, "sounds.ini"), FileLocateRules.Default);
            Configuration ini = ConfigurationManager.Instance.CreateInstance(fl);

            ConfigurationSection sect = ini["SoundList"];

            sfx = new Dictionary<string, SfxType>(sect.Count);
            
            ConfigurationSection.ValueCollection vals = sect.Values;
            foreach (string sndName in vals)
            {
                if (sndName.Length > 0)
                {
                    ConfigurationSection sfxsect;
                    if (ini.TryGetValue(sndName, out sfxsect))
                    {
                        string[] paths = sfxsect.GetPaths("Sound");
                        SfxType sf = new SfxType(this, new FileLocation(paths[0]));

                        string[] flags = sfxsect.GetStringArray("Flags", null);
                        if (flags != null) 
                        {
                            for (int i = 0; i < flags.Length; i++) 
                            {
                                flags[i] = flags[i].ToLowerInvariant();

                                switch (flags[i]) 
                                {
                                    case "loop":
                                        sf.Flags |= FMOD.MODE.LOOP_NORMAL;
                                        break;
                                }
                            }
                        }

                        sfx.Add(sndName, sf);
                    }
                }
            }
        }

        public FMOD.System SoundSystem 
        {
            get { return sndSys; }
        }

    }
}
