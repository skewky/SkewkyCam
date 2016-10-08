﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections.Generic;
using System.Text;

namespace Com.Skewky.Vlc
{
    public class VlcPlayer
    {
        private IntPtr libvlc_instance_;
        private IntPtr libvlc_media_player_;
        private double duration_;
        private string playPath;
        public VlcPlayer(string pluginPath)
        {
            string plugin_arg = "--plugin-path=" + pluginPath;
            string[] arguments = { "-I", "dummy", "--ignore-config", "--no-video-title", "--rtsp-tcp","-vvv" , plugin_arg };
            libvlc_instance_ = LibVlcAPI.libvlc_new(arguments);

            libvlc_media_player_ = LibVlcAPI.libvlc_media_player_new(libvlc_instance_);
        }
        ~VlcPlayer()
        {
            Stop();
        }
        public void Copy(VlcPlayer vlcPlayer)
        {
            libvlc_instance_ = vlcPlayer.libvlc_instance_;
            libvlc_media_player_ = vlcPlayer.libvlc_media_player_;
            duration_ = vlcPlayer.duration_;

        }
        public void setPlayInfo(PlayInfo pInfo)
        {
            if (null == pInfo)
                return;
            if (playPath != pInfo.filePath)
                PlayFile(pInfo.filePath);
            SetPlayTime(pInfo.curTime);
            SetRate(pInfo.dPlayingSpeed);
            SetVolume(pInfo.dValume);
            if (pInfo.playStatus == vlc_Sta.libvlc_Playing)
            {
                Play();
                SetPlayTime(pInfo.curTime);
            }
            else if (pInfo.playStatus == vlc_Sta.libvlc_Paused)
            {
                Pause();
            }
            else if (pInfo.playStatus == vlc_Sta.libvlc_Stopped)
            {
                Stop();
            }
        }
        public PlayInfo getPlayInfo()
        {
            PlayInfo pInfo = new PlayInfo();
            pInfo.filePath = playPath;
            pInfo.curTime = GetPlayTime();
            pInfo.playStatus = getPlayerStatus();
            pInfo.dPlayingSpeed = GetRate();
            pInfo.dValume = GetVolume();
            return pInfo;
        }
        public void SetRenderWindow(int wndHandle)
        {
            if (libvlc_instance_ != IntPtr.Zero && wndHandle != 0)
            {
                LibVlcAPI.libvlc_media_player_set_hwnd(libvlc_media_player_, wndHandle);
            }
        }

        public void PlayFile(string filePath)
        {
            IntPtr libvlc_media = LibVlcAPI.libvlc_media_new_path(libvlc_instance_, filePath);
            if (libvlc_media != IntPtr.Zero)
            {
                LibVlcAPI.libvlc_media_parse(libvlc_media);
                duration_ = LibVlcAPI.libvlc_media_get_duration(libvlc_media);

                LibVlcAPI.libvlc_media_player_set_media(libvlc_media_player_, libvlc_media);
                LibVlcAPI.libvlc_media_release(libvlc_media);

                LibVlcAPI.libvlc_media_player_play(libvlc_media_player_);
                playPath = filePath;
            }
        }
        public void PrepareFile(string filePath)
        {
            IntPtr libvlc_media = LibVlcAPI.libvlc_media_new_path(libvlc_instance_, filePath);
            if (libvlc_media != IntPtr.Zero)
            {
                LibVlcAPI.libvlc_media_parse(libvlc_media);
                duration_ = LibVlcAPI.libvlc_media_get_duration(libvlc_media);

                LibVlcAPI.libvlc_media_player_set_media(libvlc_media_player_, libvlc_media);
                LibVlcAPI.libvlc_media_release(libvlc_media);
                playPath = filePath;
                //LibVlcAPI.libvlc_media_player_play(libvlc_media_player_);
            }
        }
        public void Pause()
        {
            if (libvlc_media_player_ != IntPtr.Zero)
            {
                LibVlcAPI.libvlc_media_player_pause(libvlc_media_player_);
            }
        }

        public void Play()
        {
            if (libvlc_media_player_ != IntPtr.Zero)
            {
                LibVlcAPI.libvlc_media_player_play(libvlc_media_player_);
            }
        }
        public void Stop()
        {
            if (libvlc_media_player_ != IntPtr.Zero)
            {
                LibVlcAPI.libvlc_media_player_stop(libvlc_media_player_);
            }
        }

        public double GetPlayTime()
        {
            return LibVlcAPI.libvlc_media_player_get_time(libvlc_media_player_) / 1000.0;
        }

        public void SetPlayTime(double seekTime)
        {
            LibVlcAPI.libvlc_media_player_set_time(libvlc_media_player_, (Int64)(seekTime * 1000));
        }

        public int GetVolume()
        {
            return LibVlcAPI.libvlc_audio_get_volume(libvlc_media_player_);
        }

        public void SetVolume(int volume)
        {
            LibVlcAPI.libvlc_audio_set_volume(libvlc_media_player_, volume);
        }

        public void SetFullScreen(bool istrue)
        {
            LibVlcAPI.libvlc_set_fullscreen(libvlc_media_player_, istrue ? 1 : 0);
        }
        public void SetRate(double rate)
        {
            if (libvlc_media_player_ != IntPtr.Zero)
            {
                LibVlcAPI.libvlc_media_player_set_rate(libvlc_media_player_, (float)rate);
            }
        }
        public double GetRate()
        {
            double rate = 1;
            if (libvlc_media_player_ != IntPtr.Zero)
            {
                rate = (float)LibVlcAPI.libvlc_media_player_get_rate(libvlc_media_player_);
            }
            return rate;
        }
        public double Duration()
        {
            return duration_;
        }

        public string Version()
        {
            return LibVlcAPI.libvlc_get_version();
        }
        public bool isPlayEnded()
        {
            vlc_Sta sta = (vlc_Sta)LibVlcAPI.libvlc_media_player_get_state(libvlc_media_player_);
            return vlc_Sta.libvlc_Ended == sta;
        }
        public vlc_Sta getPlayerStatus()
        {
            vlc_Sta sta = (vlc_Sta)LibVlcAPI.libvlc_media_player_get_state(libvlc_media_player_);
            return sta;
        }
    }

    internal static class LibVlcAPI
    {
        internal struct PointerToArrayOfPointerHelper
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public IntPtr[] pointers;
        }

        public static IntPtr libvlc_new(string[] arguments)
        {
            PointerToArrayOfPointerHelper argv = new PointerToArrayOfPointerHelper();
            argv.pointers = new IntPtr[11];

            for (int i = 0; i < arguments.Length; i++)
            {
                argv.pointers[i] = Marshal.StringToHGlobalAnsi(arguments[i]);
            }

            IntPtr argvPtr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(typeof(PointerToArrayOfPointerHelper));
                argvPtr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(argv, argvPtr, false);

                return libvlc_new(arguments.Length, argvPtr);
            }
            finally
            {
                for (int i = 0; i < arguments.Length + 1; i++)
                {
                    if (argv.pointers[i] != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(argv.pointers[i]);
                    }
                }
                if (argvPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(argvPtr);
                }
            }
        }

        public static IntPtr libvlc_media_new_path(IntPtr libvlc_instance, string path)
        {
            IntPtr pMrl = IntPtr.Zero;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(path);
                pMrl = Marshal.AllocHGlobal(bytes.Length + 1);
                Marshal.Copy(bytes, 0, pMrl, bytes.Length);
                Marshal.WriteByte(pMrl, bytes.Length, 0);
                return libvlc_media_new_path(libvlc_instance, pMrl);
            }
            finally
            {
                if (pMrl != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pMrl);
                }
            }
        }

        public static IntPtr libvlc_media_new_location(IntPtr libvlc_instance, string path)
        {
            IntPtr pMrl = IntPtr.Zero;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(path);
                pMrl = Marshal.AllocHGlobal(bytes.Length + 1);
                Marshal.Copy(bytes, 0, pMrl, bytes.Length);
                Marshal.WriteByte(pMrl, bytes.Length, 0);
                return libvlc_media_new_path(libvlc_instance, pMrl);
            }
            finally
            {
                if (pMrl != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pMrl);
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        // 以下是libvlc.dll导出函数

        // 创建一个libvlc实例，它是引用计数的
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr libvlc_new(int argc, IntPtr argv);

        // 释放libvlc实例
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_release(IntPtr libvlc_instance);

        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern String libvlc_get_version();

        // 从视频来源(例如Url)构建一个libvlc_meida
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr libvlc_media_new_location(IntPtr libvlc_instance, IntPtr path);

        // 从本地文件路径构建一个libvlc_media
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr libvlc_media_new_path(IntPtr libvlc_instance, IntPtr path);

        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_release(IntPtr libvlc_media_inst);

        // 创建libvlc_media_player(播放核心)
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern IntPtr libvlc_media_player_new(IntPtr libvlc_instance);

        // 将视频(libvlc_media)绑定到播放器上
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_set_media(IntPtr libvlc_media_player, IntPtr libvlc_media);

        // 设置图像输出的窗口
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_set_hwnd(IntPtr libvlc_mediaplayer, Int32 drawable);

        // 设置播放
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_play(IntPtr libvlc_mediaplayer);

        // 设置暂停
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_pause(IntPtr libvlc_mediaplayer);

        // 设置停止
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_stop(IntPtr libvlc_mediaplayer);

        // 解析视频资源的媒体信息(如时长等)
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_parse(IntPtr libvlc_media);

        // 返回视频的时长(必须先调用libvlc_media_parse之后，该函数才会生效)
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern Int64 libvlc_media_get_duration(IntPtr libvlc_media);

        // 当前播放的时间
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern Int64 libvlc_media_player_get_time(IntPtr libvlc_mediaplayer);

        // 设置播放位置(拖动)
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_set_time(IntPtr libvlc_mediaplayer, Int64 time);

        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_release(IntPtr libvlc_mediaplayer);

        // 获取和设置音量
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int libvlc_audio_get_volume(IntPtr libvlc_media_player);

        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_audio_set_volume(IntPtr libvlc_media_player, int volume);

        // 设置全屏
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_set_fullscreen(IntPtr libvlc_media_player, int isFullScreen);

        // 设置播放速度
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern void libvlc_media_player_set_rate(IntPtr libvlc_media_player, float rate);

        // 获取播放速度
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern float libvlc_media_player_get_rate(IntPtr libvlc_media_player);

        //获取播放状态
        [DllImport("libvlc", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int libvlc_media_player_get_state(IntPtr libvlc_media_player);

    }
}