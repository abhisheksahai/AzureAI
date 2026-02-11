using MLSampleApp_ConsoleApp;

var sampleData = new PredictiveModel.ModelInput()
{
	UDI = 10,
	Product_ID = @"M14869",
	Air_temperature = 298.5F,
	Process_temperature = 309F,
	Rotational_speed = 1741F,
	Torque = 28F,
	Tool_wear = 21F,
};

var scores = PredictiveModel.PredictAllLabels(sampleData);
foreach (var score in scores)
{
	Console.WriteLine($"{score.Key,-40}{score.Value,-40}");
}

//Load model and predict output
var result = PredictiveModel.Predict(sampleData);
Console.WriteLine($"Machine Failure: {result.Machine_failure}");
