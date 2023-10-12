import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {IonicModule} from '@ionic/angular';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HomePage} from './home.page';

import {HomePageRoutingModule} from './home-routing.module';
import {ProductsComponent} from "../products/products.component";
import {CreateBoxComponent} from "../create-box/create-box.component";
import {HttpClientModule} from "@angular/common/http";
import {BoxcardComponent} from "../boxcard/boxcard.component";
import {DetailsComponent} from "../details/details.component";
import {UpdateBoxComponent} from "../update-box/update-box.component";


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    HomePageRoutingModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  exports: [
    HomePage,
    ProductsComponent
  ],
  declarations: [HomePage, ProductsComponent, CreateBoxComponent, BoxcardComponent, DetailsComponent, UpdateBoxComponent]
})
export class HomePageModule {
}
