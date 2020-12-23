import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EventService } from '../shared/api-generated/api-generated';


@Component({
  selector: 'app-landingpage-event-invitation',
  templateUrl: './landingpage-event-invitation.component.html',
  styleUrls: ['./landingpage-event-invitation.component.scss']
})
export class LandingpageEventInvitationComponent implements OnInit {

  constructor(private  eventservice: EventService, private route: ActivatedRoute) {

  }

  ngOnInit(): void {
    this.sendAnswere();
  }

  sendAnswere(){
    debugger;
    const id = +this.route.snapshot.paramMap.get('id');
    const contactId = +this.route.snapshot.paramMap.get('contactId');
    const organizationId = +this.route.snapshot.paramMap.get('organizationId');
    const state = +this.route.snapshot.paramMap.get('state');
    this.eventservice.postInvitationResponse(id, contactId, organizationId, state).subscribe(data => {});
  }
}