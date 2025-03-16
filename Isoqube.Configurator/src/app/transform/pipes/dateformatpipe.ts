import { Pipe, PipeTransform } from '@angular/core';
import moment from 'moment';

@Pipe({
    name: 'dateonly',
    standalone: true
})
export class DateOnlyPipe implements PipeTransform {
    transform(isoDate: string, format?: string): string {
        if (!isoDate || isoDate === '') { return isoDate; };
        return moment(isoDate).format(format ?? 'MMMM Do YYYY')
    }
}

@Pipe({
    name: 'datefromnow',
    standalone: true
})
export class DatefromNowPipe implements PipeTransform {
    transform(isoDate: string, format?: string): string {
        if (!isoDate || isoDate === '') { return isoDate; };
        return moment(isoDate).fromNow();
    }
}