using Newtonsoft.Json;
using OpenAssets.Examples.API;

const string apiKey = "<Enter API Key Here>";
const string api = "http://api-sandbox.open-assets.co.uk";

var client = new HttpClient();
client.BaseAddress = new Uri(api);
client.DefaultRequestHeaders.Add("x-api-key", apiKey);

DateTimeOffset? maxUpdatedTimestampUtc = null;
var mostRecentTimestampUtc = await GetMostRecentTimestampUtc();
var skip = 0;
var take = 50;

while (true)
{
    var response = JsonConvert.DeserializeObject<WarningSearchResponse>(
        await client.GetStringAsync($"/warnings?take={take}&skip={skip}")
    );

    // todo - process warning updates

    if (!response!.Results.Any())
    {
        break;
    }

    maxUpdatedTimestampUtc ??= response.Results.Max(x => x.UpdatedTimestampUtc);

    if (mostRecentTimestampUtc > response.Results.Min(x => x.UpdatedTimestampUtc) || take > response.Results.Count)
    {
        break;
    }
    skip += take;
}

if (maxUpdatedTimestampUtc.HasValue)
{
    await UpdateLastRunTimestampUtc(maxUpdatedTimestampUtc.Value);
}


Task UpdateLastRunTimestampUtc(DateTimeOffset mostRecentTimestampUtc)
{
    // todo - persist the most recent warning timestamp for use on the next run.
    return Task.CompletedTask;
}

Task<DateTimeOffset> GetMostRecentTimestampUtc()
{
    // todo - replace with logic to get the last run timestamp from storage, such as a SQL Db.
    return Task.FromResult(DateTimeOffset.MinValue);
}