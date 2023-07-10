using DictionaryInterpolation;
using Newtonsoft.Json;

var specificFeedbackItem = new SpecificFeedbackItem("reference", 11, "hola");

Console.WriteLine(specificFeedbackItem?.Detail);

var serializedSfeedbackItem = JsonConvert.SerializeObject(specificFeedbackItem);

Console.WriteLine(serializedSfeedbackItem);

var feedbackItem = JsonConvert.DeserializeObject<FeedbackItem>(serializedSfeedbackItem);

Console.WriteLine(feedbackItem?.Detail);

Console.ReadLine();

public record FeedbackItem
{
    public FeedbackItem(string type, string instance, params string[] extensions)
    {
        Type = type;
        Instance = instance;
    }

    [JsonProperty("Detail")]
    public string DetailResource => Resource.ResourceManager.GetString(Type + "Detail")!;

    [JsonIgnore]
    public virtual string Detail
    {
        get
        {
            var detail = DetailResource;

            if (detail == null)
            {
                return Resource.ResourceManager.GetString("DefaultDetail")!;
            }

            detail = detail.Replace("{" + nameof(Instance) + "}", Instance);

            if(Extensions == null)
            {
                return detail;
            }

            foreach (var extension in Extensions)
            {
                detail = detail.Replace("{" + extension.Key + "}", extension.Value.ToString());
            }

            return detail;
        }
    }

    public string Type { get; set; }

    public string Instance { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object>? Extensions { get; set; }
}

public record SpecificFeedbackItem : FeedbackItem
{
    public SpecificFeedbackItem(string reference, int number, string text)
        : base("20201", reference)
    {
        CustomNumber = number;
        CustomText = text;
    }

    public override string Detail
    {
        get
        {
            var detail = base.Detail;
            detail = detail.Replace("{" + nameof(CustomNumber) + "}", CustomNumber.ToString());
            detail = detail.Replace("{" + nameof(CustomText) + "}", CustomText.ToString());
            return detail;
        }
    }

    public int CustomNumber { get; set; }

    public string CustomText { get; set; }
}
