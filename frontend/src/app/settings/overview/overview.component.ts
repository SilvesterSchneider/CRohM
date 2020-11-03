import { Component, OnInit } from '@angular/core';
import { JwtService } from 'src/app/shared/jwt.service';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss']
})
export class OverviewComponent implements OnInit {

  constructor(private jwt: JwtService) { }

  public ngOnInit(): void {
  }

  isAdmin(): boolean {
    return this.jwt.isAdmin();
  }
}
