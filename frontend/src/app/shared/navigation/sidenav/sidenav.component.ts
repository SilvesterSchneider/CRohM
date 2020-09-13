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
  activeLinkIndex = -1;
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
      }, {
          label: 'Veranstaltungen',
          link: './events',
          index: 2
      },
    ];
   }

  ngOnInit(): void {
    this.router.events.subscribe((res) => {
      this.activeLinkIndex = this.navLinks.indexOf(this.navLinks.find(tab => tab.link === '.' + this.router.url));
    });
  }

  public isLoggedIn() {
    return this.jwtService.isLoggedIn();
  }
}
