using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace ArtGenerator.Models
{
    /// <summary>
    /// Base object with everything regarding a painting.
    /// </summary>
    public class Painting : BindableBase
    {
        /// <summary>
        /// Background color for painting
        /// </summary>
        private Brush _background;
        public Brush Background
        {
            get { return _background; }
            set { SetProperty(ref _background, value); }
        }

        /// <summary>
        /// List of all the shapes in the painting
        /// </summary>
        private ObservableCollection<PaintingShape> _shapes = new();
        public ObservableCollection<PaintingShape> Shapes { get { return _shapes; } set { SetProperty(ref _shapes, value); } }

        /// <summary>
        /// Version/iteration of painting, f.e. version 2 is the
        /// painting created after the first selection by the user.
        /// </summary>
        private int _iterationsSurvived = 1;
        public int MayLiveUntil = 0;
        public int MutationCount = 0;

        public string GetIterationsSurvived()
        {
            return (_iterationsSurvived - 2).ToString();
        }

        public int GetLikes()
        {
            return _iterationsSurvived - 2;
        }

        public void SetIterationsSurvived(int value)
        {
            _iterationsSurvived = value;
        }

        public void UpdateIterationsSurvived(int currentIteration)
        {
            _iterationsSurvived++;
            MayLiveUntil = currentIteration + _iterationsSurvived;
        }
    }
}
