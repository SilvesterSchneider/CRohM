import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  public clickCounter = 0;
  public name = '';


  constructor() { }

  ngOnInit() {
  }

  countClick() {
    this.clickCounter++;
  }

}
