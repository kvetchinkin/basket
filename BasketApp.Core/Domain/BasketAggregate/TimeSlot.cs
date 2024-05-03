using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace BasketApp.Core.Domain.BasketAggregate;

/// <summary>
///     Период доставки
/// </summary>
public class TimeSlot : Entity<int>
{
    public static readonly TimeSlot Morning = new(1, nameof(Morning).ToLowerInvariant(), 6, 12);
    public static readonly TimeSlot Midday = new(2, nameof(Midday).ToLowerInvariant(), 12, 17);
    public static readonly TimeSlot Evening = new(3, nameof(Evening).ToLowerInvariant(), 17, 24);
    public static readonly TimeSlot Night = new(4, nameof(Night).ToLowerInvariant(), 0, 6);

    /// <summary>
    /// Ошибки, которые может возвращать сущность
    /// </summary>
    public static class Errors
    {
        public static Error TimeSlotIsWrong()
        {
            return new($"{nameof(TimeSlot).ToLowerInvariant()}.is.wrong",
                $"Не верное значение. Допустимые значения: {nameof(TimeSlot).ToLowerInvariant()}: {string.Join(",", List().Select(s => s.Name))}");
        }
    }

    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Начало периода
    /// </summary>
    public int Start { get; private set; }

    /// <summary>
    /// Конец периода
    /// </summary>
    public int End { get; private set; }

    /// <summary>
    /// Ctr
    /// </summary>
    [ExcludeFromCodeCoverage]
    private TimeSlot()
    {
    }

    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <param name="name">Название</param>
    /// <param name="start">Начало периода, часы</param>
    /// <param name="end">Конец периода, часы</param>
    private TimeSlot(int id, string name, int start, int end)
    {
        Id = id;
        Name = name;
        Start = start;
        End = end;
    }

    /// <summary>
    /// Список всех значений списка
    /// </summary>
    /// <returns>Значения списка</returns>
    public static IEnumerable<TimeSlot> List()
    {
        yield return Morning;
        yield return Midday;
        yield return Evening;
        yield return Night;
    }

    public static Result<TimeSlot, Error> FromName(string name)
    {
        var state = List()
            .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        if (state == null) return Errors.TimeSlotIsWrong();
        return state;
    }

    public static Result<TimeSlot, Error> From(int id)
    {
        var state = List().SingleOrDefault(s => s.Id == id);
        if (state == null) return Errors.TimeSlotIsWrong();
        return state;
    }
}