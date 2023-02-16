import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { LogLevel } from './logging.service';

@Injectable({
  providedIn: 'root',
})
export class ConfigService {
  public readonly current$ = new BehaviorSubject<Config>({
    logLevelForBackend: LogLevel.debug,
    logLevelForConsole: LogLevel.debug,
  });

  constructor() {}
}

export interface Config {
  logLevelForBackend: LogLevel;
  logLevelForConsole: LogLevel;
}
