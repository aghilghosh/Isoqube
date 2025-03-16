import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";
import { ITopic, Configuration } from "../models/http";

@Injectable({
  providedIn: 'root'
})

export class OrchestrationService {
  
  constructor(private http: HttpClient) { }

  runConfiguration(runConfiguration: any): Observable<any> {
    return this.http.post(`${environment.API_ENDPOINT}/api/configuration/run`, runConfiguration);
  }

  getAllruns(): Observable<any> {
    return this.http.get(`${environment.API_ENDPOINT}/api/configuration/run/all`);
  }

  getAllTopics(): Observable<ITopic[]> {
    return this.http.get<ITopic[]>(`${environment.API_ENDPOINT}/api/registeredtopics`);
  }  

  addConfiguration(configuration: Configuration) {    
    return this.http.post<Configuration>(`${environment.API_ENDPOINT}/api/configuration`, configuration);
  }

  getConfiguration(): Observable<Configuration[]> {    
    return this.http.get<Configuration[]>(`${environment.API_ENDPOINT}/api/configuration/all`);
  }
}

