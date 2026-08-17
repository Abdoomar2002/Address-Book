/**
 * Formats a Date as a plain yyyy-MM-dd calendar date using its local parts.
 *
 * `toISOString()` converts to UTC first, so a date picked at local midnight in a
 * negative-offset timezone would come out as the previous day.
 */
export function toIsoDate(date: Date): string {
  const month = `${date.getMonth() + 1}`.padStart(2, '0');
  const day = `${date.getDate()}`.padStart(2, '0');

  return `${date.getFullYear()}-${month}-${day}`;
}
