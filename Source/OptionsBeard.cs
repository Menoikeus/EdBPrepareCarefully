using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace EdB.PrepareCarefully {
    public class OptionsBeard {
        protected List<BeardDef> maleBeards = new List<BeardDef>();
        protected List<BeardDef> femaleBeards = new List<BeardDef>();
        protected List<BeardDef> noGenderBeards = new List<BeardDef>();
        protected List<Color> BeardColors = new List<Color>();
        public List<BeardDef> MaleBeards {
            get {
                return maleBeards;
            }
            set {
                maleBeards = value;
            }
        }
        public List<BeardDef> FemaleBeards {
            get {
                return femaleBeards;
            }
            set {
                femaleBeards = value;
            }
        }
        public List<BeardDef> NoGenderBeards {
            get {
                return noGenderBeards;
            }
            set {
                noGenderBeards = value;
            }
        }
        public void AddBeard(BeardDef def) {
            if (def.styleGender == StyleGender.Male) {
                maleBeards.Add(def);
            }
            else if (def.styleGender == StyleGender.Female) {
                femaleBeards.Add(def);
            }
            else {
                maleBeards.Add(def);
                femaleBeards.Add(def);
                noGenderBeards.Add(def);
            }
        }
        public List<BeardDef> GetBeards(Gender gender) {
            if (gender == Gender.Male) {
                return maleBeards;
            }
            else if (gender == Gender.Female) {
                return femaleBeards;
            }
            else {
                return noGenderBeards;
            }
        }
        public List<Color> Colors {
            get {
                return BeardColors;
            }
            set {
                BeardColors = value;
            }
        }
        public void Sort() {
            Comparison<BeardDef> sorter = (BeardDef x, BeardDef y) => {
                if (x.label == null) {
                    return -1;
                }
                else if (y.label == null) {
                    return 1;
                }
                else {
                    return x.label.CompareTo(y.label);
                }
            };
            maleBeards.Sort(sorter);
            femaleBeards.Sort(sorter);
            noGenderBeards.Sort(sorter);
        }
    }
}
