using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Crom.Controls
{
    /// <summary>
    /// Implementation of a tab button
    /// </summary>
    internal class TabButton
    {
        #region Fields.

        private const int SpaceImageText = 5;
        private const int ImageDimension = 16;

        private Control _container = null;
        private ITitleData _titleData = null;
        private Rectangle _bounds = new Rectangle();
        private bool _selected = false;
        private bool _hoover = false;
        private bool _showSelection = false;
        private Color _notSelectedColor = Color.DarkGray;
        private Color _selectedColor = Color.Black;
        private Color _selectedBorderColor = Color.FromArgb(75, 75, 111);
        private Color _selectedBackColor1 = Color.FromArgb(255, 242, 200);
        private Color _selectedBackColor2 = Color.FromArgb(255, 215, 157);

        #endregion Fields.

        #region Instance.

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container">container</param>
        /// <param name="titleData">title data</param>
        public TabButton(Control container, ITitleData titleData)
        {
            _container = container;
            _titleData = titleData;
        }

        #endregion Instance.

        #region Public section.

        /// <summary>
        /// Color of the text when the button is not selected
        /// </summary>
        public Color NotSelectedColor
        {
            get { return _notSelectedColor; }
            set { _notSelectedColor = value; }
        }

        /// <summary>
        /// Color of the text when the button is selected
        /// </summary>
        public Color SelectedColor
        {
            get { return _selectedColor; }
            set { _selectedColor = value; }
        }

        /// <summary>
        /// Selected border color
        /// </summary>
        public Color SelectedBorderColor
        {
            get { return _selectedBorderColor; }
            set { _selectedBorderColor = value; }
        }

        /// <summary>
        /// Low gradient color of the text when the button is selected
        /// </summary>
        public Color SelectedBackColor1
        {
            get { return _selectedBackColor1; }
            set { _selectedBackColor1 = value; }
        }

        /// <summary>
        /// High gradient color of the text when the button is selected
        /// </summary>
        public Color SelectedBackColor2
        {
            get { return _selectedBackColor2; }
            set { _selectedBackColor2 = value; }
        }

        /// <summary>
        /// Reset the size of the tab button to empty.
        /// </summary>
        /// <remarks>
        /// It also remove the selection flag.
        /// </remarks>
        public void Reset()
        {
            _bounds.X = 0;
            _bounds.Y = 0;
            _bounds.Width = 0;
            _bounds.Height = 0;

            Selected = false;
        }

        /// <summary>
        /// Title data required to draw the button
        /// </summary>
        public ITitleData TitleData
        {
            get { return _titleData; }
        }

        /// <summary>
        /// Bounds of the tab buttom
        /// </summary>
        public Rectangle Bounds
        {
            get { return _bounds; }
        }

        /// <summary>
        /// Flag indicating if the button is selected or not.
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        /// <summary>
        /// Flag indicating if the button is under mouse cursor
        /// </summary>
        public bool Hoover
        {
            get { return _hoover; }
            set { _hoover = value; }
        }

        /// <summary>
        /// Flag indicating if the button should draw border and gradient background when is selected
        /// </summary>
        public bool ShowSelection
        {
            get { return _showSelection; }
            set { _showSelection = value; }
        }

        /// <summary>
        /// Draw the tab button
        /// </summary>
        /// <param name="bounds">bounds of the tab button</param>
        /// <param name="font">font for drawing the button text</param>
        /// <param name="vertical">true if the buttone is vertical</param>
        /// <param name="g">graphics</param>
        public void Draw(Rectangle bounds, Font font, bool vertical, Graphics g)
        {
            var drawColor = NotSelectedColor;
            var drawFont = font;

            if (Selected)
            {
                drawFont = new Font(font, FontStyle.Bold);
                drawColor = SelectedColor;
            }
            else
            {
                drawFont = new Font(font, FontStyle.Regular);
            }

            using (drawFont)
            {
                _bounds.Location = bounds.Location;
                _bounds.Size = bounds.Size;

                var icon = _titleData.Icon;
                var text = _titleData.Title();

                var imageBounds = new Rectangle(0, 0, ImageDimension, ImageDimension);
                var textSize = g.MeasureString(text, drawFont);
                var averageTextSize = g.MeasureString("x", drawFont);
                textSize.Width += averageTextSize.Width;

                var textPosition = ImageDimension + 2 * SpaceImageText;
                var textHeight = (int)textSize.Height;
                var textWidth = 0;

                if (vertical)
                {
                    textWidth = Math.Max(0, Math.Min((int)textSize.Width, bounds.Height - textPosition));
                    _bounds.Height = textPosition + textWidth;
                }
                else
                {
                    textWidth = Math.Max(0, Math.Min((int)textSize.Width, bounds.Width - textPosition));
                    _bounds.Width = textPosition + textWidth;
                }

                g.SetClip(_bounds);

                if (Selected && ShowSelection)
                {
                    using (var backBrush = new LinearGradientBrush(_bounds, SelectedBackColor1, SelectedBackColor2, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(backBrush, _bounds);
                    }

                    using (var borderPen = new Pen(SelectedBorderColor))
                    {
                        g.DrawRectangle(borderPen, _bounds.Left, _bounds.Top, _bounds.Width - 1, _bounds.Height - 1);
                    }
                }

                if (Hoover)
                {
                    if (Hoover)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(255, 233, 186)))
                        {
                            g.FillRectangle(brush, _bounds);
                        }
                    }

                    using (var pen = new Pen(Color.FromArgb(75, 75, 111)))
                    {
                        g.DrawRectangle(pen, _bounds.Left, _bounds.Top, _bounds.Width - 1, _bounds.Height - 1);
                    }
                }

                using (var bmp = icon.ToBitmap())
                {
                    if (vertical)
                    {
                        imageBounds.Y = bounds.Y + SpaceImageText;
                        imageBounds.X = bounds.X + Math.Max(0, (bounds.Width - ImageDimension) / 2 - 0);
                    }
                    else
                    {
                        imageBounds.X = bounds.X + SpaceImageText;
                        imageBounds.Y = bounds.Y + Math.Max(0, (bounds.Height - ImageDimension) / 2 - 0);
                    }
                    g.DrawImage(bmp, imageBounds);
                }

                var textBounds = new Rectangle(0, 0, textWidth, textHeight);

                var textX = bounds.X + 2 * SpaceImageText + ImageDimension;
                var textY = bounds.Y + Math.Max(0, (bounds.Height - textHeight) / 2 - 0);

                if (vertical)
                {
                    textX = bounds.X + Math.Max(0, (bounds.Width - textHeight) / 2 - 0);
                    textY = bounds.Y + 2 * SpaceImageText + ImageDimension;
                }

                if (textBounds.Width > 0 && textBounds.Height > 0)
                {
                    using (var textImg = new Bitmap(textBounds.Width, textBounds.Height))
                    {
                        using (var gt = Graphics.FromImage(textImg))
                        {
                            gt.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                            TextRenderer.DrawText(gt, text, drawFont, textBounds, drawColor,
                               TextFormatFlags.ModifyString | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine |
                               TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
                        }

                        if (vertical)
                        {
                            g.TranslateTransform(textX + textHeight, textY);
                            g.RotateTransform(90);

                            g.DrawImage(textImg, 0, 0);

                            g.RotateTransform(-90);
                            g.TranslateTransform(-(textX + textHeight), -textY);
                        }
                        else
                        {
                            g.DrawImage(textImg, textX, textY);
                        }
                    }
                }
            }
        }

        #endregion Public section.
    }
}
