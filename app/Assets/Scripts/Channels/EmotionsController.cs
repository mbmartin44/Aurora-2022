using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neuro;
using Neuro.Native;

public class EmotionsController
{
    EmotionsAnalyzer emotionsAnalyzer = null;

    public Action<EmotionsSample> onEmotionalStateChanged;
    public Action<IEmotionsAnalyzerState> onAnalyzerStateChanged;


    public EmotionsController(Device device) 
    {
        emotionsAnalyzer = new EmotionsAnalyzer(device);
        emotionsAnalyzer.AnalyzerStateChanged += onAnalyzerStateRecieved;
        emotionsAnalyzer.EmotionalStateChanged += onEmotionalStateRecieved;
        emotionsAnalyzer.Weights = new EmotionsWeights() { Alpha = 1, Beta = 1, Delta = 0, Theta = 0 };


        device.Execute(Command.StartSignal);
        emotionsAnalyzer?.Calibrate();
    }

    private void onEmotionalStateRecieved(object sender, EmotionsSample e)
    {
        onEmotionalStateChanged?.Invoke(e);
    }

    private void onAnalyzerStateRecieved(object sender, IEmotionsAnalyzerState e)
    {
        onAnalyzerStateChanged?.Invoke(e);
    }

    public void CloseChannel(Device device)
    {
        device.Execute(Command.StopSignal);

        emotionsAnalyzer.AnalyzerStateChanged -= onAnalyzerStateRecieved;
        emotionsAnalyzer.EmotionalStateChanged -= onEmotionalStateRecieved;
        emotionsAnalyzer.Dispose();
        emotionsAnalyzer = null;

        onEmotionalStateChanged = null;
        onAnalyzerStateChanged = null;
    }
}
