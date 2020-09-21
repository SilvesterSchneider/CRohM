export function sortDatesDesc(date1: string, date2: string) {
    return new Date(date2).getTime() - new Date(date1).getTime();
}

