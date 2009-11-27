using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VirtualBicycle.Config;
using VirtualBicycle.ConfigModel;
using VirtualBicycle.IO;
using VirtualBicycle.Graphics;

namespace VirtualBicycle.Sound
{
    public sealed class Themes : Singleton, IUpdatable
    {
        static Themes singleton;

        public static Themes Instance
        {
            get { return singleton; }
        }
        public static void Initialize(FMOD.System sndSys, AudioConfigs aus)
        {
            singleton = new Themes(sndSys, aus);
        }

        public sealed class Track : IDisposable
        {
            string name;
            FMOD.Sound sound;
            FMOD.Channel chanel;
            FMOD.System sndSystem;

            bool normal;
            bool repeat;

            bool isPlaying;
            bool disposed;

            Themes themes;

            public Track(Themes thms, FMOD.System sndSys, ConfigurationSection sect)
            {
                themes = thms;
                sndSystem = sndSys;

                name = sect.GetUIString("Name", "GUI:Blank");

                normal = sect.GetBool("Normal", true);
                repeat = sect.GetBool("Repeat", false);

                string[] fn = sect.GetPaths("File");

                FileLocation fileLoc = FileSystem.Instance.Locate(fn, FileLocateRules.Music);

                FMOD.CREATESOUNDEXINFO sndInfo = new FMOD.CREATESOUNDEXINFO();
                sndInfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(sndInfo);
                sndInfo.fileoffset = (uint)fileLoc.Offset;
                sndInfo.length = (uint)fileLoc.Size;


                FMOD.MODE mode = FMOD.MODE._2D | FMOD.MODE.DEFAULT | FMOD.MODE.HARDWARE | FMOD.MODE.CREATESTREAM;

                //if (repeat)
                //    mode |= FMOD.MODE.LOOP_NORMAL;

                if (!fileLoc.IsInArchive)
                {
                    sndSys.createSound(fileLoc.Path, mode, ref sound);
                }
                else
                {
                    //string fp = fileLoc.Path;                    
                    //sndSys.createSound(fp.Substring(0, fp.LastIndexOf(Path.DirectorySeparatorChar)), mode, ref sndInfo, ref sound);
                    sndSys.createSound(FileSystem.GetArchivePath(fileLoc.Path), mode, ref sndInfo, ref sound);
                }

            }

            public void SetVolume(float value)
            {
                if (chanel != null)
                {
                    chanel.setVolume(value);
                }
            }

            public void Play()
            {
                if (sound != null)
                {
                    sndSystem.playSound(FMOD.CHANNELINDEX.REUSE, sound, true, ref chanel);
                    chanel.setVolume(themes.audioCons.ScoreVolume);
                    //chanel.setDelay(FMOD.DELAYTYPE.MAX, 1000, 0);
                    chanel.setPaused(false);
                    isPlaying = true;
                }
            }

            public void Stop()
            {
                if (isPlaying)
                {
                    if (chanel != null)
                    {
                        chanel.stop();
                    }
                    isPlaying = false;
                }
            }

            public TimeSpan Length
            {
                get
                {
                    if (sound != null)
                    {
                        uint l = 0;
                        sound.getLength(ref l, FMOD.TIMEUNIT.MS);
                        return new TimeSpan(0, 0, 0, 0, (int)l);
                    }
                    return TimeSpan.Zero;
                }
            }

            public string Name
            {
                get { return name; }
            }


            public bool Repeat
            {
                get { return repeat; }
            }

            #region IDisposable 成员

            public void Dispose()
            {
                if (!disposed)
                {
                    if (sound != null)
                    {
                        sound.release();
                    }
                    sndSystem = null;
                    themes = null;
                    disposed = true;
                    //GC.SuppressFinalize(this);
                }
                else
                    throw new ObjectDisposedException(this.ToString());
            }

            #endregion

            ~Track()
            {
                if (!disposed)
                    Dispose();
            }
        }


        int currentIndex;

        float remainingTime;


        List<Track> tracks;
        Dictionary<string, Track> tracksName;

        Track currentTrack;

        Track menu;
        Track loading;
        Track score;
        Track credits;

        AudioConfigs audioCons;
        FMOD.System sndSys;

        bool isLoaded;



        private Themes(FMOD.System sndSys, AudioConfigs aus)
        {
            this.sndSys = sndSys;
            this.audioCons = aus;
        }

        public List<KeyValuePair<string, string>> GetTrackInformation()
        {
            List<KeyValuePair<string, string>> res = new List<KeyValuePair<string, string>>(tracks.Count);

            for (int i = 0; i < tracks.Count; i++)
                res.Add(new KeyValuePair<string, string>(tracks[i].Name, tracks[i].Length.ToString()));
            return res;
        }

        public bool IsRepeating
        {
            get;
            set;
        }

        void Play(Track tck)
        {
            IsRepeating = tck.Repeat;
            remainingTime = (float)tck.Length.TotalMilliseconds * 0.001f;

            Stop();

            currentTrack = tck;
            tck.Play();
        }
        void PlayNewSong()
        {
            if (!IsRepeating)
            {
                currentIndex = Randomizer.GetRandomInt(tracks.Count - 4) + 4;
            }

            Track tck = tracks[currentIndex];
            Play(tck);
        }

        public void Update(float dt)
        {
            remainingTime -= dt;
            if (remainingTime < 0)
            {
                PlayNewSong();
            }
        }

        public void SetVolume(float value)
        {
            currentTrack.SetVolume(value);
        }

        void Load()
        {
            FileLocation fl = FileSystem.Instance.Locate(Path.Combine(Paths.Configs, "theme.ini"), FileLocateRules.Default);
            Configuration ini = ConfigurationManager.Instance.CreateInstance(fl);

            ConfigurationSection sect = ini["SongList"];

            tracks = new List<Track>(sect.Count);
            tracksName = new Dictionary<string, Track>(sect.Count);

            ConfigurationSection.ValueCollection vals = sect.Values;
            foreach (string sndName in vals)
            {
                if (sndName.Length > 0)
                {
                    ConfigurationSection tsect;
                    if (ini.TryGetValue(sndName, out tsect))
                    {
                        Track tck = new Track(this, sndSys, tsect);

                        string sn = sndName.ToLowerInvariant();

                        switch (sn)
                        {
                            case "menu":
                                menu = tck;
                                break;
                            case "loading":
                                loading = tck;
                                break;
                            case "score":
                                score = tck;
                                break;
                            case "credits":
                                credits = tck;
                                break;
                            default:
                                tracks.Add(tck);
                                break;
                        }

                        tracksName.Add(sndName, tck);
                    }
                }
            }

            tracks.TrimExcess();
            isLoaded = true;
        }

        public void Play()
        {
            if (!isLoaded)
            {
                Load();
            }
            IsRepeating = false;
            PlayNewSong();
        }
        public void Play(string name)
        {
            if (!isLoaded)
            {
                Load();
            }
            Play(tracksName[name]);
        }
        public void Play(int i)
        {
            if (!isLoaded)
            {
                Load();
            }
            Play(tracks[i]);
        }


        public void Stop()
        {
            if (currentTrack != null)
            {
                currentTrack.Stop();
            }
        }


        protected override void dispose()
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                tracks[i].Stop();
                tracks[i].Dispose();
            }
            tracks = null;

            singleton = null;
        }
    }
}
