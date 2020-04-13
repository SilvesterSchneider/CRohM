import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { OrganizationDto } from '../shared/api-generated/api-generated';
import { ORGANIZATIONS } from './mock-organizations';

@Injectable({
    providedIn: 'root',
})
export class OrganizationsMockService {
    static nextId = 4;
    private orgas: OrganizationDto[] = ORGANIZATIONS;
    private organizations: BehaviorSubject<OrganizationDto[]> = new BehaviorSubject<OrganizationDto[]>(ORGANIZATIONS);

    constructor() { }

    getOrganizationsMock() {
        return this.organizations.asObservable();
    }

    delete(id: number) {
        const index: number = this.orgas.findIndex(x => x.id === id);
        if (index !== -1) {
            this.orgas.splice(index, 1);
            this.organizations.next(this.orgas);
        }
    }
}
