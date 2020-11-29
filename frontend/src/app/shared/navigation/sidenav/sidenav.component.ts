import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { JwtService } from '../../jwt.service';

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss']
})
export class SidenavComponent implements OnInit {
  navLinks: any[];
  constructor(private jwtService: JwtService, private router: Router) {
    this.navLinks = [
      {
          label: 'Kontakte',
          link: './contacts',
          index: 0
      }, {
          label: 'Organisationen',
          link: './organizations',
          index: 1
      },  {
          label: 'Kalender',
          link: './calendar',
          index: 2
      }, {
          label: 'Veranstaltungen',
          link: './events',
          index: 3
      }, {
        label: 'Statistiken',
        link: './statistics',
        index: 4
    }
    ];
   }

  ngOnInit(): void {
  }

  public isLoggedIn() {
    return this.jwtService.isLoggedIn();
  }
}
