import { Component, OnInit } from '@angular/core';
import { ContactService, OrganizationService, ContactDto, OrganizationDto } from '../shared/api-generated/api-generated';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  public contacts: ContactDto[] = new Array<ContactDto>();
  public organizations: OrganizationDto[] = new Array<OrganizationDto>();

  constructor(private contactsService: ContactService,
    private organizationService: OrganizationService) { }

  ngOnInit() {
    this.contactsService.getAll().subscribe(x =>
      {
        let index = x.length - 1;
        if (index > -1) {
          this.contacts.push(x[index]);
        }
        if (index > 0) {
          this.contacts.push(x[index - 1]);
        }
      });
    this.organizationService.get().subscribe(x =>
      {
        let index = x.length - 1;
        if (index > -1) {
          this.organizations.push(x[index]);
        }
        if (index > 0) {
          this.organizations.push(x[index - 1]);
        }
      });
  }
}
