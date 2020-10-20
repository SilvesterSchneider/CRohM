import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { ContactService } from '../shared/api-generated/api-generated';


@Injectable({
  providedIn: 'root'
})
export class ContactsIdResolver2Service {
	constructor(private approveContact: ContactService, private router: Router) {}

	resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): void {
    const id = +route.paramMap.get('id');
    this.approveContact.approveContact(id).subscribe();
	}
}


