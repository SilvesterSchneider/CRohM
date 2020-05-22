
import { Injectable } from '@angular/core';
import {
    Router, Resolve,
    RouterStateSnapshot,
    ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of, EMPTY } from 'rxjs';
import { mergeMap, take } from 'rxjs/operators';
import { EventDto } from '../shared/api-generated/api-generated';
import { EventService } from '../shared/api-generated/api-generated';

@Injectable({
    providedIn: 'root',
})
export class EventsDetailResolverService implements Resolve<EventDto> {
    constructor(private cs: EventService, private router: Router) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<EventDto> | Observable<never> {
        const id = +route.paramMap.get('id');
        return this.cs.getById(id).pipe(
            take(1),
            mergeMap(event => {
                if (event) {
                    return of(event);
                } else { // id not found
                    return EMPTY;
                }
            })
        );
    }
}
