// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Mediapipe.Unity
{
  public class ConnectionAnnotation_Custom : LineAnnotation_Custom
  {
    private Connection_Custom _currentTarget;

    public bool isEmpty => _currentTarget == null;

    public void Draw(Connection_Custom target)
    {
      _currentTarget = target;

      if (ActivateFor(_currentTarget))
      {
        Draw(_currentTarget.start.gameObject, _currentTarget.end.gameObject);
      }
    }

    public void Redraw()
    {
      Draw(_currentTarget);
    }

    protected bool ActivateFor(Connection_Custom target)
    {
      if (target == null || !target.start.isActiveInHierarchy || !target.end.isActiveInHierarchy)
      {
        SetActive(false);
        return false;
      }
      SetActive(true);
      return true;
    }
  }
}
