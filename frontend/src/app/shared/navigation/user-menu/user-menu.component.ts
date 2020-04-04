import { Component, OnInit } from '@angular/core';
import { JwtService } from '../../jwt.service';

@Component({
  selector: 'app-user-menu',
  templateUrl: './user-menu.component.html',
  styleUrls: ['./user-menu.component.scss']
})
export class UserMenuComponent implements OnInit {

  constructor(private jwtService: JwtService) { }

  ngOnInit(): void {
  }

  public logout() {
    this.jwtService.logout();
    this.jwtService.goToLogin();
  }

}
