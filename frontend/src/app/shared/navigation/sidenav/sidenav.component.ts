import { Component, OnInit } from '@angular/core';
import { JwtService } from '../../jwt.service';

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss']
})
export class SidenavComponent implements OnInit {

  constructor(private jwtService: JwtService) { }

  ngOnInit(): void {
  }

  public isLoggedIn() {
    return this.jwtService.isLoggedIn();
  }

}
