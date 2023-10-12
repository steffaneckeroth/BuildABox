import { Component, OnInit } from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {firstValueFrom} from "rxjs";
import {Box} from "../models";
import {environment} from "../../environments/environment";


@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss'],
})
export class ProductsComponent implements OnInit {
  boxes: Box[] = [];

  constructor(private http: HttpClient) {
    this.http = http;
  }

  ngOnInit() {
    this.getAllProducts();
  }

  async getAllProducts() {
    const call = this.http.get<Box[]>(environment.apiBaseUrl +"products");
    const result = await firstValueFrom<Box[]>(call);
    this.boxes = result;
  }

  async handleSearch(event: any) {

    const emptyQuery = event.target.value;

    const options = {
      params : new HttpParams().set('searchQuery', emptyQuery)
    }

    const call = this.http.get<Box[]>(environment.apiBaseUrl + "products/filter", options);
    const result = await firstValueFrom<Box[]>(call);

    this.boxes = result;
  }

}
