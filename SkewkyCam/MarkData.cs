﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Skewky.Cam
{
    [Serializable]
    public class MarkData
    {
        string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        bool bFavourite;
        public bool Favourite
        {
            get { return bFavourite; }
            set { bFavourite = value; }
        }
        bool bToDelete;
        public bool ToDelete
        {
            get { return bToDelete; }
            set { bToDelete = value; }
        }
        bool bPrivate;
        public bool Private
        {
            get { return bPrivate; }
            set { bPrivate = value; }
        }
        public bool Describ
        {
            get { return !string.IsNullOrEmpty(description); }
        }
        public string MDStr
        {
            get 
            { 
                string res = "";
                res += bFavourite ? "L" : " ";
                res += bToDelete ? "D" : " ";
                res += bPrivate ? "P" : " ";
                res += Describ ? "N" : " ";
                return res;
            }
        }
    }
}