// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Mediapipe.Unity
{
  public class Connection_Custom
  {
    public readonly HierarchicalAnnotation_Custom start;
    public readonly HierarchicalAnnotation_Custom end;

    public Connection_Custom(HierarchicalAnnotation_Custom start, HierarchicalAnnotation_Custom end)
    {
      this.start = start;
      this.end = end;
    }
  }
}
