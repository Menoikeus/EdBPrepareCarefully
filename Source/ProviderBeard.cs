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
    public class ProviderBeard {
        protected Dictionary<ThingDef, OptionsBeard> BeardLookup = new Dictionary<ThingDef, OptionsBeard>();
        protected OptionsBeard humanlikeBeards;
        protected OptionsBeard noBeard = new OptionsBeard();
        public ProviderBeard() {
        }
        public ProviderAlienRaces AlienRaceProvider {
            get; set;
        }
        public List<BeardDef> GetBeards(CustomPawn pawn) {
            return GetBeards(pawn.Pawn.def, pawn.Gender);
        }
        public List<BeardDef> GetBeards(ThingDef raceDef, Gender gender) {
            OptionsBeard Beards = GetBeardsForRace(raceDef);
            return Beards.GetBeards(gender);
        }
        public OptionsBeard GetBeardsForRace(CustomPawn pawn) {
            return GetBeardsForRace(pawn.Pawn.def);
        }
        public OptionsBeard GetBeardsForRace(ThingDef raceDef) {
            OptionsBeard Beards;
            if (BeardLookup.TryGetValue(raceDef, out Beards)) {
                return Beards;
            }
            Beards = InitializeBeards(raceDef);
            if (Beards == null) {
                if (raceDef != ThingDefOf.Human) {
                    return GetBeardsForRace(ThingDefOf.Human);
                }
                else {
                    return null;
                }
            }
            else {
                BeardLookup.Add(raceDef, Beards);
                return Beards;
            }
        }
        protected OptionsBeard InitializeBeards(ThingDef raceDef) {
            AlienRace alienRace = AlienRaceProvider.GetAlienRace(raceDef);
            if (alienRace == null) {
                return HumanlikeBeards;
            }
            if (!alienRace.HasHair) {
                return noBeard;
            }
            // If the alien race does not have a limited set of Beards, then we'll try to re-use the humanlike Beard options.
            if (alienRace.HairTags == null) {
                // If the selection of Beards is the same and the alien race has no custom color generator, then
                // we can just re-use the humanlike Beard options.
                if (alienRace.HairColors == null) {
                    return HumanlikeBeards;
                }
                // If there is a custom color generator, then we make a copy of the humanlike Beard options--preserving
                // the BeardDef lists--but we replace the color list.
                else {
                    OptionsBeard humanBeards = HumanlikeBeards;
                    OptionsBeard humanBeardsWithColors = new OptionsBeard();
                    humanBeardsWithColors.MaleBeards = humanBeards.MaleBeards;
                    humanBeardsWithColors.FemaleBeards = humanBeards.FemaleBeards;
                    humanBeardsWithColors.NoGenderBeards = humanBeards.NoGenderBeards;
                    humanBeardsWithColors.Colors = alienRace.HairColors.ToList();
                    return humanBeardsWithColors;
                }
            }
            OptionsBeard result = new OptionsBeard();
            foreach (BeardDef BeardDef in DefDatabase<BeardDef>.AllDefs.Where((BeardDef def) => {
                foreach (var tag in def.styleTags) {
                    if (alienRace.HairTags.Contains(tag)) {
                        return true;
                    }
                }
                return false;
            })) {
                result.AddBeard(BeardDef);
            }
            
            if (alienRace.HairColors != null) {
                result.Colors = alienRace.HairColors.ToList();
            }
            
            result.Sort();
            return result;
        }
        protected OptionsBeard HumanlikeBeards {
            get {
                if (humanlikeBeards == null) {
                    humanlikeBeards = InitializeHumanlikeBeards();
                }
                return humanlikeBeards;
            }
        }
        protected OptionsBeard InitializeHumanlikeBeards() {
            HashSet<string> nonHumanBeardTags = new HashSet<string>();
            // This was meant to remove alien race-specific Beard defs from those available when customizing non-aliens.
            // However, there's no way to distinguish between Beard tags that are ONLY for aliens vs. the non-alien
            // Beard defs that are also allow for aliens.  This makes the logic below fail.  Instead, we'll include
            // all Beard def (both alien and non-alien) in the list of available Beards for non-aliens.
            // TODO: Implement filtering in the Beard selection to make it easier to find appropriate Beards when there
            // are a lot of mods that add Beards.
            /*
            IEnumerable<ThingDef> alienRaces = DefDatabase<ThingDef>.AllDefs.Where((ThingDef def) => {
                return def.race != null && ProviderAlienRaces.IsAlienRace(def);
            });
            foreach (var alienRaceDef in alienRaces) {
                AlienRace alienRace = AlienRaceProvider.GetAlienRace(alienRaceDef);
                if (alienRace == null) {
                    continue;
                }
                if (alienRace.BeardTags != null) {
                    foreach (var tag in alienRace.BeardTags) {
                        nonHumanBeardTags.Add(tag);
                    }
                }
            }
            */
            OptionsBeard result = new OptionsBeard();
            foreach (BeardDef BeardDef in DefDatabase<BeardDef>.AllDefs.Where((BeardDef def) => {
                foreach (var tag in def.styleTags) {
                    if (nonHumanBeardTags.Contains(tag)) {
                        return false;
                    }
                }
                return true;
            })) {
                result.AddBeard(BeardDef);
            }
            result.Sort();

            // Set up default Beard colors
            result.Colors.Add(new Color(0.2f, 0.2f, 0.2f));
            result.Colors.Add(new Color(0.31f, 0.28f, 0.26f));
            result.Colors.Add(new Color(0.25f, 0.2f, 0.15f));
            result.Colors.Add(new Color(0.3f, 0.2f, 0.1f));
            result.Colors.Add(new Color(0.3529412f, 0.227451f, 0.1254902f));
            result.Colors.Add(new Color(0.5176471f, 0.3254902f, 0.1843137f));
            result.Colors.Add(new Color(0.7568628f, 0.572549f, 0.3333333f));
            result.Colors.Add(new Color(0.9294118f, 0.7921569f, 0.6117647f));

            return result;
        }
    }
}
