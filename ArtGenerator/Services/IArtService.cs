using ArtGenerator.Models;

namespace ArtGenerator.Services
{
    public interface IArtService
    {
        public void SetPanelWidthAndHeight(int panelWidth, int panelHeight);
        public Painting GenerateArt(Painting preferredPanel, Painting otherPanel);
        public void SaveGenerationData(Painting leftPanel, Painting rightPanel);
        public void LoadGenerationData(string serializedJsonData, out Painting LeftPanel, out Painting RightPanel);
        public bool CanUseAlgorithm();
        public int GetIterationNumber();
        public void ResetAlgorithm();
    }
}
