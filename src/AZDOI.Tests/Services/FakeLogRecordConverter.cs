using Microsoft.Extensions.Logging.Testing;

namespace AZDOI.Tests.Services;

public class FakeLogRecordConverter : WriteOnlyJsonConverter<FakeLogRecord>
{
    public override void Write(VerifyJsonWriter writer, FakeLogRecord record)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(record.Level.ToString("F"));
        writer.WriteValue(record.Message);
        if (!string.IsNullOrWhiteSpace(record.Category))
        {
            writer.WriteMember(record, record.Category, "Category");
        }
        if (record.Scopes
                .Where(scope => scope != null)
                .ToArray() is { Length: > 0 } scopes)
        {
            writer.WriteMember(record, scopes, "Scopes");
        }
        if (record.Id.Id != 0)
        {
            writer.WriteMember(record, record.Id, "EventId");
        }
        if (record.State != null)
        {
            writer.WriteMember(record, record.State, "State");
        }
        if (record.Exception != null)
        {
            writer.WriteMember(record, record.Exception, "Exception");
        }
        writer.WriteEndObject();
    }
}
