import { Injectable } from '@angular/core';
import { Router, Resolve, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of, EMPTY } from 'rxjs';
import { mergeMap, take } from 'rxjs/operators';
import { ContactDto } from '../shared/api-generated/api-generated';
import { ContactService } from '../shared/api-generated/api-generated';

@Injectable({
	providedIn: 'root'
})
export class ContactsDetailResolverService implements Resolve<ContactDto> {
	constructor(private cs: ContactService, private router: Router) {}

	resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<ContactDto> | Observable<never> {
		const id = +route.paramMap.get('id');

		return this.cs.getById(id).pipe(
			take(1),
			mergeMap((contact) => {
				if (contact) {
					return of(contact);
				} else {
					// id not found
					return EMPTY;
				}
			})
		);
	}
}
