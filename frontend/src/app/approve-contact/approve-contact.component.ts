import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ContactService } from '../shared/api-generated/api-generated';

@Component({
  selector: 'app-approve-contact',
  templateUrl: './approve-contact.component.html',
  styleUrls: ['./approve-contact.component.scss']
})
export class ApproveContactComponent implements OnInit {

  
  constructor(private approveContact: ContactService, private route: ActivatedRoute){

  }

  approved: boolean = false;

  ngOnInit(): void {
    this.Approve();
  }

  Approve(){
    const id = +this.route.snapshot.paramMap.get('id');
    this.approveContact.approveContact(id).subscribe((data) => this.approved = true,
    (error) => this.approved = false);
  }
}
