using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vision.BL.Model;
using Vision.Wpf.Model;

namespace Vision.Wpf
{
    public class Shared
    {
        public static LinkView AddNewLink(Window owner)
        {
            var newLink = new Link
            {
                Name = "Noname",
            };
            var newLinkView = Global.Mapper.Map<LinkView>(newLink);
            newLinkView.Tag = newLink;
            Shared.EditLink(owner, newLinkView);
            return newLinkView;
        }

        public static void EditLink(Window owner, LinkView linkView)
        {
            var dlg = new EditLinkWindow(linkView)
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            dlg.ShowDialog();
            CopyToLinkBehind(linkView);
        }

        public static void ToggleFavorite(LinkView linkView)
        {
            linkView.IsFavorite = !linkView.IsFavorite;
            linkView.ImageSource = linkView.IsFavorite ? Global.FavoriteStarUri : "";
            CopyToLinkBehind(linkView);
        }

        private static void CopyToLinkBehind(LinkView linkView)
        {
            Global.Mapper.Map(linkView, linkView.Tag as Link);
        }

    }
}
