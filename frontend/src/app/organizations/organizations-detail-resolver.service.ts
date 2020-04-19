
import { Injectable } from '@angular/core';
import {
    Router, Resolve,
    RouterStateSnapshot,
    ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of, EMPTY } from 'rxjs';
import { mergeMap, take } from 'rxjs/operators';
import { OrganizationDto } from '../shared/api-generated/api-generated';
import { OrganizationService } from '../shared/api-generated/api-generated';

@Injectable({
    providedIn: 'root',
})
export class OrganizationsDetailResolverService implements Resolve<OrganizationDto> {
    constructor(private os: OrganizationService, private router: Router) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<OrganizationDto> | Observable<never> {
        const id = +route.paramMap.get('id');

        return this.os.getById(id).pipe(
            take(1),
            mergeMap(organization => {
                if (organization) {
                    return of(organization);
                } else { // id not found
                    return EMPTY;
                }
            })
        );
    }
}
