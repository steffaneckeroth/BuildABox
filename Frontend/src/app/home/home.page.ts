import { Component } from '@angular/core';
import {Router} from "@angular/router";
import { MenuController } from "@ionic/angular";



@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
})
export class HomePage {


  constructor(private router: Router, private menu: MenuController) {}

  goToCreateSite() {
    this.router.navigate(['createBox'])
    this.menu.close();
  }

  goToProductsSite() {
    this.router.navigate(['products'])
   this.menu.close();
  }





}
