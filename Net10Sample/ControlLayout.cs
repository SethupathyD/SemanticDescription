using Microsoft.Maui.Layouts;

namespace Net10Sample
{
    public static class LayoutHelper
    {
        public static int ColumnCount = 3;
        public static int RowCount = 3;
        public static double ColumnWidth = 100;
    }

    public class MatrixContainer : BaseLayout
    {
        public MatrixContainer() { }

        internal override Size MeasureContent(double widthConstraint, double heightConstraint)
        {
            if (Children.Count == 0)
            {
                for (int i = 0; i < LayoutHelper.RowCount; i++)
                    Add(new MatrixRow(i));
            }

            foreach (var child in Children)
            {
                child.Measure(LayoutHelper.ColumnCount * 100, 50);
                (child as MatrixRow)?.CrossPlatformMeasure(widthConstraint, heightConstraint);
            }
            return new Size(LayoutHelper.ColumnCount * 100, LayoutHelper.RowCount * 50);
        }

        internal override Size ArrangeContent(Rect bounds)
        {
            var dy = 0;
            foreach (var child in Children)
            {
                child.Arrange(new Rect(bounds.X, dy, LayoutHelper.ColumnCount * 100, 50));
                (child as MatrixRow)?.CrossPlatformArrange(bounds);
                dy += 50;
            }
            return bounds.Size;
        }

        protected override ILayoutManager CreateLayoutManager() => new BaseLayoutManager(this);
    }

    public class MatrixRow : BaseLayout
    {
        public MatrixRow(int index)
        {
            Index = index;
        }

        internal int Index = 0;

        internal override Size MeasureContent(double widthConstraint, double heightConstraint)
        {
            if (Children.Count == 0)
            {
                for (int i = 0; i < LayoutHelper.ColumnCount; i++)
                    Add(new MatrixCell(i));
            }

            for (int i = 0; i < Children.Count; i++)
            {
                var width = i == 0 ? LayoutHelper.ColumnWidth : 100;
                Children[i].Measure(width, 50);
            }
            return new Size(LayoutHelper.ColumnCount * 100, 50);
        }

        internal override Size ArrangeContent(Rect bounds)
        {
            double dx = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                var width = i == 0 ? LayoutHelper.ColumnWidth : 100;
                Children[i].Arrange(new Rect(dx, bounds.Y, width, 50));
                dx += width;
            }
            return bounds.Size;
        }

        protected override ILayoutManager CreateLayoutManager() => new BaseLayoutManager(this);
    }

    public class MatrixCell : ContentView
    {
        public MatrixCell(int index)
        {
            Index = index;

            var lbl = new Label
            {
                Text = $"Index{index}",
            };

            AutomationProperties.SetIsInAccessibleTree(this, true);
            this.RemoveBinding(SemanticProperties.DescriptionProperty);

            var cellValue = lbl.Text;
            this.SetBinding(
                SemanticProperties.DescriptionProperty,
                new Binding(".", source: $" {Index} {cellValue}")
            );

            Content = lbl;
        }

        internal int Index = 0;
    }

    public abstract class BaseLayout : Layout
    {
        internal abstract Size ArrangeContent(Rect bounds);
        internal abstract Size MeasureContent(double widthConstraint, double heightConstraint);
    }

    internal class BaseLayoutManager : LayoutManager
    {
        BaseLayout layout;

        internal BaseLayoutManager(BaseLayout layout) : base(layout)
        {
            this.layout = layout;
        }

        public override Size ArrangeChildren(Rect bounds) => layout.ArrangeContent(bounds);
        public override Size Measure(double widthConstraint, double heightConstraint) => layout.MeasureContent(widthConstraint, heightConstraint);
    }
}
