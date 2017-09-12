namespace Crom.Controls
{
   /// <summary>
   /// Enumerates how can be docked a control inside <see cref="DockContainer">DockContainer</see>
   /// </summary>
   public enum zDockMode
   {
      /// <summary>
      /// The control can't be docked.
      /// </summary>
      None        = 0,
      /// <summary>
      /// The control can be docked on left side
      /// </summary>
      Left        = 1,
      /// <summary>
      /// The control can be docked on top side
      /// </summary>
      Top         = 2,
      /// <summary>
      /// The control can be docked on right side
      /// </summary>
      Right       = 4,
      /// <summary>
      /// The control can be docked on bottom side
      /// </summary>
      Bottom      = 8,
      /// <summary>
      /// The control can fill the container
      /// </summary>
      Fill        = 16,
      /// <summary>
      /// The control can be docked horizontally, on <see cref="Left"/> or on <see cref="Right"/>
      /// </summary>
      Horizontally = Left | Right,
      /// <summary>
      /// The control can be docked vertically, on <see cref="Top"/> or on <see cref="Bottom"/>
      /// </summary>
      Vertically = Top | Bottom,
      /// <summary>
      /// The control can be docked on the sides of the container.
      /// The sides are <see cref="Left"/>, <see cref="Right"/>, <see cref="Top"/>, <see cref="Bottom"/>
      /// </summary>
      Sides = Horizontally | Vertically,
      /// <summary>
      /// The control can be docked on all <see cref="Sides"/> and also can <see cref="Fill"/> the container
      /// </summary>
      All = Sides | Fill
   }
}
