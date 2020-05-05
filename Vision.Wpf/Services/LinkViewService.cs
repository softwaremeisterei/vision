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
    public class LinkViewService
    {
        public LinkView AddNewLink(Window owner, string url = null)
        {
            var newLink = new Link
            {
                Name = "_Noname_",
                Url = url
            };
            var newLinkView = Global.Mapper.Map<LinkView>(newLink);
            newLinkView.Tag = newLink;
            EditLink(owner, newLinkView);
            return newLinkView;
        }

        public void EditLink(Window owner, LinkView linkView)
        {
            var dlg = new EditLinkWindow(linkView)
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            dlg.ShowDialog();
            CopyToLinkBehind(linkView);
        }

        public void ToggleFavorite(LinkView linkView)
        {
            linkView.IsFavorite = !linkView.IsFavorite;
            CopyToLinkBehind(linkView);
        }

        private void CopyToLinkBehind(LinkView linkView)
        {
            Global.Mapper.Map(linkView, linkView.Tag as Link);
        }

    }
}
