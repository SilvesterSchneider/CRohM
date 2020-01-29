
import { Injectable } from '@angular/core';
import {
    Router, Resolve,
    RouterStateSnapshot,
    ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of, EMPTY } from 'rxjs';
import { mergeMap, take } from 'rxjs/operators';
import { Contact } from './contacts.model';
import { ContactsService } from './contacts.service';


@Injectable({
    providedIn: 'root',
})
export class ContactsDetailResolverService implements Resolve<Contact> {
    constructor(private cs: ContactsService, private router: Router) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Contact> | Observable<never> {
        const id = route.paramMap.get('id');

        return this.cs.getContact(id).pipe(
            take(1),
            mergeMap(contact => {
                if (contact) {
                    return of(contact);
                } else { // id not found
                    return EMPTY;
                }
            })
        );
    }
}
