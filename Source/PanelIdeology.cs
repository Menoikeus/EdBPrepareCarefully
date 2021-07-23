using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace EdB.PrepareCarefully {
    public class PanelIdeology : PanelBase {
        public delegate void ChangeIdeoHandler(Ideo ideo);

        public event ChangeIdeoHandler IdeoChanged;

        protected Rect RectScrollFrame;
        protected Rect RectScrollView;
        protected Rect RectIdeologyIcon;
        protected Rect RectIdeoLabelContainer;

        protected Rect RectButtonLeft;
        protected Rect RectButtonRight;

        protected List<Ideo> ideos;

        public PanelIdeology() {
            ideos = Verse.Find.IdeoManager.IdeosListForReading;
        }

        public override string PanelHeader {
            get {
                return "Ideology";
            }
        }

        protected void UpdateIdeo(Ideo ideo) {
            IdeoChanged(ideo);
        }

        public override void Resize(Rect rect) {
            base.Resize(rect);

            float panelPadding = 10;
            Vector2 contentSize = new Vector2(PanelRect.width - panelPadding * 2, BodyRect.height - panelPadding);

            RectScrollFrame = new Rect(panelPadding, BodyRect.y, contentSize.x, contentSize.y);
            RectScrollView = new Rect(0, 0, RectScrollFrame.width, RectScrollFrame.height);
            float ideologyLabelHeight = 20;
            float ideologyLabelPadding = 4;
            float ideologyIconWidth = RectScrollView.height - ideologyLabelHeight - ideologyLabelPadding;
            float iconOffset = (RectScrollView.width - ideologyIconWidth) / 2;
            RectIdeologyIcon = new Rect(iconOffset, 0, ideologyIconWidth, ideologyIconWidth);
            RectIdeoLabelContainer = new Rect(RectScrollView.x, RectIdeologyIcon.height + ideologyLabelPadding, RectScrollView.width, ideologyLabelHeight);

            float halfButtonHeight = 8;
            float buttonTopOffset = RectIdeologyIcon.y + ideologyIconWidth / 2 - halfButtonHeight;

            float buttonPadding = 8;
            RectButtonLeft = new Rect(RectIdeologyIcon.x - 17 - buttonPadding, buttonTopOffset, 16, 16);
            RectButtonRight = new Rect(RectIdeologyIcon.x + RectIdeologyIcon.width + 1 + buttonPadding, buttonTopOffset, 16, 16);
        }

        protected override void DrawPanelContent(State state) {
            base.DrawPanelContent(state);
            CustomPawn customPawn = state.CurrentPawn;

            GUI.BeginGroup(RectScrollFrame);

            // Draw ideo information
            if (customPawn.OriginalIdeo == null) {
                GUI.color = Style.ColorText;
                Widgets.Label(RectScrollView.InsetBy(1, 0, 0, 0), "EdB.PC.Panel.Ideology.None".Translate());
            }
            else {
                GUI.color = customPawn.OriginalIdeo.Color;
                GUI.DrawTexture(RectIdeologyIcon, customPawn.OriginalIdeo.Icon);
            }

            int originalIdeoIndex = ideos.IndexOf(customPawn.OriginalIdeo);

            // Draw the decrement button.
            if (originalIdeoIndex == 0) {
                GUI.color = Style.ColorButtonDisabled;
            }
            else {
                Style.SetGUIColorForButton(RectButtonLeft);
            }
            GUI.DrawTexture(RectButtonLeft, Textures.TextureButtonPrevious);
            if (originalIdeoIndex != 0) {
                if (Widgets.ButtonInvisible(RectButtonLeft, false)) {
                    SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                    int ideoIndex = originalIdeoIndex - 1;
                    UpdateIdeo(ideos[ideoIndex]);
                }
            }

            // Draw the increment button.
            if (originalIdeoIndex == ideos.Count - 1) {
                GUI.color = Style.ColorButtonDisabled;
            }
            else {
                Style.SetGUIColorForButton(RectButtonRight);
            }
            GUI.DrawTexture(RectButtonRight, Textures.TextureButtonNext);
            if (originalIdeoIndex != ideos.Count - 1) {
                if (Widgets.ButtonInvisible(RectButtonRight, false)) {
                    SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                    int ideoIndex = originalIdeoIndex + 1;
                    UpdateIdeo(ideos[ideoIndex]);
                }
            }

            // Draw the ideo label
            GameFont saveFont = Text.Font;
            Text.Font = GameFont.Small;
            Vector2 ideoLabelSize = Text.CalcSize(state.CurrentPawn.OriginalIdeo.name);
            Text.Font = saveFont;
            Rect RectIdeoLabel = new Rect(RectIdeoLabelContainer.MiddleX() - ideoLabelSize.x / 2,
                RectIdeoLabelContainer.y, ideoLabelSize.x, ideoLabelSize.y);
            GUI.color = Color.white;
            Widgets.Label(RectIdeoLabel, state.CurrentPawn.OriginalIdeo.name);
            GUI.EndGroup();
        }
    }
}
