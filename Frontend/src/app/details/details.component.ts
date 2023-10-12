import {Component, inject, Injectable, Input, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {catchError, firstValueFrom, Observable, throwError} from "rxjs";
import {ActivatedRoute, Router} from "@angular/router";
import {environment} from "../../environments/environment";
import {DataService} from "../data.service";
import {Box} from "../models";


@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss'],
})
export class DetailsComponent {

  box: Box | any = {}

  productId: number | undefined;

  constructor(private http: HttpClient,
              private router: Router,
              private route: ActivatedRoute,
              public dataService: DataService) {
    this.getBoxById();
  }



  async getBoxById() {
    const map = await firstValueFrom(this.route.paramMap)
    const id = map.get('id')
    const call = this.http.get<Box>(environment.apiBaseUrl + "products/" + id);
    this.box = await firstValueFrom<Box>(call);
    this.dataService.currentBox = this.box;
  }


  goBack() {
    this.router.navigate(['products/']).then(() => window.location.reload());
  }

  goToEdit() {
    this.router.navigate(['edit'])
  }



  deleteBox(productID: any) {

    if (confirm("Are you sure you want to delete " + this.box.title)) {

      console.log("you tried to delete id: " + productID);

      this.http.delete(environment.apiBaseUrl + "products/" + productID).subscribe();

      this.goBack();

    }
  }


}
