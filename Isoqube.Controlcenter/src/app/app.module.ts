import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { OrchestrationComponent } from './components/orchestration/orchestration.component';
import { SearchOrganizationPipe } from './transform/pipes/filter';
import { ConfigurationComponent } from './components/configuration/configuration.component';
import { DefaultComponent } from './components/default/default.component';
import { HeaderComponent } from './components/header/header.component';
import { UserSessionComponent } from './components/usersession/usersession.component';
import { AuthGuard } from './services/auth.service';
import { AppRoutingModule } from './app-routing.module';
import { ModalModule, BsModalService } from 'ngx-bootstrap/modal';
import { DatefromNowPipe } from '../../src/app/transform/pipes/dateformatpipe';

@NgModule({
  declarations: [
    AppComponent,
    UserSessionComponent,
    HeaderComponent,
    DefaultComponent,
    OrchestrationComponent,
    ConfigurationComponent,
    SearchOrganizationPipe
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule,
    ModalModule.forRoot(),
    DatefromNowPipe
  ],
  providers: [AuthGuard, BsModalService],
  bootstrap: [AppComponent]
})
export class AppModule { }
